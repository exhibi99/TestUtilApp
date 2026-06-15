using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

/*
 * 빌드 조건:
 *   1. Basler pylon 7.5 설치 (Basler_pylon_7.5.0.15658.exe)
 *   2. 프로젝트 참조에 Basler.Pylon.dll 추가
 *      경로: C:\Program Files\Basler\pylon 7\Development\lib\dotnet\Basler.Pylon.dll
 *   3. 프로젝트 속성 → 빌드 → 조건부 컴파일 기호에  BASLER_PYLON  추가
 */

#if BASLER_PYLON
using Basler.Pylon;
#endif

namespace TestUtilApp.CameraLight
{
#if BASLER_PYLON

    public class BaslerCamera : ICamera
    {
        // ── Pylon 객체 ──────────────────────────────────────────────
        private Camera            _camera;
        private PixelDataConverter _converter = new PixelDataConverter();
        private readonly Version   _sfnc2     = new Version(2, 0, 0);
        private Version            _sfncVer;

        // ── 이미지 상태 ──────────────────────────────────────────────
        private Action<Bitmap> _liveCallback;
        private bool           _isLive;

        // ── 캐시 파라미터 ────────────────────────────────────────────
        private int    _width;
        private int    _height;
        private string _pixelFormat;

        // ── ICamera ─────────────────────────────────────────────────

        public bool IsConnected => _camera?.IsOpen ?? false;

        /// <summary>
        /// identifier : 카메라 시리얼 번호 (예: "12345678").
        /// 비어있으면 연결된 첫 번째 카메라에 자동 연결.
        /// </summary>
        public void Connect(string identifier)
        {
            Disconnect();

            if (string.IsNullOrWhiteSpace(identifier))
            {
                _camera = new Camera();
            }
            else
            {
                // 시리얼로 카메라 검색
                _camera = null;
                foreach (var info in CameraFinder.Enumerate())
                {
                    if (info[CameraInfoKey.SerialNumber] == identifier)
                    {
                        _camera = new Camera(info);
                        break;
                    }
                }
                if (_camera == null)
                    throw new Exception($"Serial '{identifier}' 카메라를 찾을 수 없습니다.");
            }

            _camera.CameraOpened += Configuration.AcquireContinuous;
            _camera.Open();

            _sfncVer = _camera.GetSfncVersion();
            RefreshImageParams();

            // 프레임 콜백 등록
            _camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;
            _camera.Parameters[PLCamera.AcquisitionFrameRateEnable].TrySetValue(true);
        }

        public void Disconnect()
        {
            if (_camera == null) return;

            StopLive();
            if (_camera.StreamGrabber.IsGrabbing)
                _camera.StreamGrabber.Stop();

            _camera.StreamGrabber.ImageGrabbed -= OnImageGrabbed;

            if (_camera.IsOpen)
                _camera.Close();

            _camera.Dispose();
            _camera = null;
        }

        public void SetExposure(double microseconds)
        {
            if (!IsConnected) return;
            double v = microseconds;
            if (_sfncVer < _sfnc2)
                TrySetFloat(PLCamera.ExposureTimeAbs, ref v);
            else
                TrySetFloat(PLCamera.ExposureTime, ref v);
        }

        public void SetGain(double gain)
        {
            if (!IsConnected) return;
            double v = gain;
            if (_sfncVer < _sfnc2)
                TrySetFloat(PLCamera.GainAbs, ref v);
            else
                TrySetFloat(PLCamera.Gain, ref v);
        }

        public void SetTriggerMode(TriggerMode mode)
        {
            if (!IsConnected) return;
            switch (mode)
            {
                case TriggerMode.FreeRun:
                    _camera.Parameters[PLCamera.TriggerMode].TrySetValue("Off");
                    break;

                case TriggerMode.Software:
                    _camera.Parameters[PLCamera.TriggerMode].TrySetValue("On");
                    _camera.Parameters[PLCamera.TriggerSource].TrySetValue("Software");
                    break;

                case TriggerMode.Hardware:
                    _camera.Parameters[PLCamera.TriggerMode].TrySetValue("On");
                    _camera.Parameters[PLCamera.TriggerSource].TrySetValue("Line1");
                    break;
            }
        }

        public (GrabResult result, Bitmap image) GrabSingle(int timeoutMs = 3000)
        {
            if (!IsConnected) return (GrabResult.Error, null);

            bool wasLive = _isLive;
            if (wasLive) StopLive();

            // FreeRun + OneByOne 으로 단발 촬영
            _camera.Parameters[PLCamera.TriggerMode].TrySetValue("Off");
            _camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByUser);

            Bitmap bmp = null;
            GrabResult result = GrabResult.Timeout;

            if (_camera.StreamGrabber.RetrieveResult(timeoutMs, out IGrabResult grabResult,
                TimeoutHandling.Return))
            {
                using (grabResult)
                {
                    if (grabResult.GrabSucceeded)
                    {
                        bmp    = ConvertToBitmap(grabResult);
                        result = GrabResult.Success;
                    }
                    else
                    {
                        result = GrabResult.Error;
                    }
                }
            }

            _camera.StreamGrabber.Stop();

            if (wasLive) StartLive(_liveCallback);
            return (result, bmp);
        }

        public void StartLive(Action<Bitmap> onFrame)
        {
            if (!IsConnected || _isLive) return;
            _liveCallback = onFrame;
            _isLive       = true;
            _camera.StreamGrabber.Start(GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
        }

        public void StopLive()
        {
            if (!_isLive) return;
            _isLive = false;
            try { _camera?.StreamGrabber.Stop(); } catch { }
        }

        // ── ImageGrabbed 콜백 ────────────────────────────────────────

        private void OnImageGrabbed(object sender, ImageGrabbedEventArgs e)
        {
            try
            {
                using (var grabResult = e.GrabResult)
                {
                    if (!grabResult.GrabSucceeded || !_isLive) return;
                    var bmp = ConvertToBitmap(grabResult);
                    _liveCallback?.Invoke(bmp);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BaslerCamera] OnImageGrabbed: {ex.Message}");
            }
        }

        // ── 비트맵 변환 ──────────────────────────────────────────────

        private Bitmap ConvertToBitmap(IGrabResult grabResult)
        {
            int w = grabResult.Width;
            int h = grabResult.Height;

            // BGRA8 로 통일 변환
            _converter.OutputPixelFormat = PixelType.BGRA8packed;

            int stride = w * 4;
            byte[] converted = new byte[stride * h];
            _converter.Convert(converted, grabResult);

            var bmp     = new Bitmap(w, h, PixelFormat.Format32bppRgb);
            var bmpData = bmp.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppRgb);

            Marshal.Copy(converted, 0, bmpData.Scan0, converted.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        // ── 헬퍼 ─────────────────────────────────────────────────────

        private void RefreshImageParams()
        {
            _width       = (int)_camera.Parameters[PLCamera.Width].GetValue();
            _height      = (int)_camera.Parameters[PLCamera.Height].GetValue();
            _pixelFormat = _camera.Parameters[PLCamera.PixelFormat].GetValue();
        }

        private void TrySetFloat(FloatName param, ref double value)
        {
            double min = _camera.Parameters[param].GetMinimum();
            double max = _camera.Parameters[param].GetMaximum();
            double v   = Math.Max(min, Math.Min(max, value));
            _camera.Parameters[param].TrySetValue(v, FloatValueCorrection.ClipToRange);
            value = v;
        }

        public void Dispose() => Disconnect();
    }

#else

    // ── BASLER_PYLON 미정의 시 빌드 통과용 ──────────────────────────────
    public class BaslerCamera : ICamera
    {
        public bool IsConnected { get; private set; }

        public void Connect(string identifier)
            => throw new NotSupportedException(
                "Basler pylon 7.5 SDK를 설치하고 Basler.Pylon.dll 을 참조에 추가한 뒤\n" +
                "프로젝트 빌드 기호에 BASLER_PYLON 을 추가하세요.");

        public void Disconnect()       { IsConnected = false; }
        public void SetExposure(double us)           { }
        public void SetGain(double gain)             { }
        public void SetTriggerMode(TriggerMode mode) { }

        public (GrabResult result, Bitmap image) GrabSingle(int timeoutMs = 3000)
            => (GrabResult.Error, null);

        public void StartLive(Action<Bitmap> onFrame) { }
        public void StopLive()                        { }
        public void Dispose()                         { }
    }

#endif
}
