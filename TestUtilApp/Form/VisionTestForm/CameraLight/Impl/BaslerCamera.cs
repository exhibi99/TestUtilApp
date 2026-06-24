using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

/*
 * 빌드 조건:
 *   1. packages\Basler.Pylon.NET.x64.8.0.0.10\lib\native\Basler.Pylon.dll (로컬 패키지)
 *   2. 프로젝트 속성 → 빌드 → 조건부 컴파일 기호에  BASLER_PYLON  추가
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
        private Action<Bitmap>  _liveCallback;
        private volatile bool   _isLive;   // 배경 스레드에서 즉시 인식되도록 volatile

        // ── 캐시 파라미터 ────────────────────────────────────────────
        private int    _width;
        private int    _height;
        private string _pixelFormat;

        // ── ICamera ─────────────────────────────────────────────────

        public bool   IsConnected    => _camera?.IsOpen ?? false;
        public string ConnectedSerial { get; private set; } = "";

        /// <summary>
        /// identifier : 카메라 시리얼 번호 (예: "12345678").
        /// 비어있으면 연결된 첫 번째 카메라에 자동 연결.
        /// </summary>
        public void Connect(string identifier)
        {
            Disconnect();
            ConnectedSerial = "";

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

            // 실제 연결된 카메라 시리얼 저장 (identifier="" 로 열었을 때도 확정됨)
            ConnectedSerial = _camera.CameraInfo[CameraInfoKey.SerialNumber];
            _sfncVer = _camera.GetSfncVersion();
            RefreshImageParams();

            // 프레임 콜백 등록
            _camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;
            _camera.Parameters[PLCamera.AcquisitionFrameRateEnable].TrySetValue(true);
        }

        public void Disconnect()
        {
            ConnectedSerial = "";
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

            using (var grabResult = _camera.StreamGrabber.RetrieveResult(timeoutMs, TimeoutHandling.Return))
            {
                if (grabResult != null)
                {
                    if (grabResult.GrabSucceeded)
                    {
                        bmp    = ConvertToBitmap(grabResult);
                        result = GrabResult.Success;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[BaslerCamera] GrabSingle failed: {grabResult.ErrorDescription} (code={grabResult.ErrorCode})");
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
            // Live 미리보기는 항상 FreeRun — Software/Hardware 트리거 상태에선 OnImageGrabbed 호출 안됨
            _camera.Parameters[PLCamera.TriggerMode].TrySetValue("Off");
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
            // ProvidedByUser(GrabSingle) 모드에서는 RetrieveResult가 직접 결과를 처리하므로 무시
            if (!_isLive) return;
            try
            {
                using (var grabResult = e.GrabResult)
                {
                    if (!grabResult.GrabSucceeded) return;
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

    // ── BASLER_PYLON 미정의 시 → StubCamera 로 동작 (UI 테스트용) ─────────
    public class BaslerCamera : ICamera
    {
        private readonly StubCamera _stub = new StubCamera();

        public bool   IsConnected    => _stub.IsConnected;
        public string ConnectedSerial => _stub.ConnectedSerial;
        public void Connect(string identifier)       => _stub.Connect(identifier);
        public void Disconnect()                     => _stub.Disconnect();
        public void SetExposure(double us)           => _stub.SetExposure(us);
        public void SetGain(double gain)             => _stub.SetGain(gain);
        public void SetTriggerMode(TriggerMode mode) => _stub.SetTriggerMode(mode);

        public (GrabResult result, Bitmap image) GrabSingle(int timeoutMs = 3000)
            => _stub.GrabSingle(timeoutMs);

        public void StartLive(Action<Bitmap> onFrame) => _stub.StartLive(onFrame);
        public void StopLive()                        => _stub.StopLive();
        public void Dispose()                         => _stub.Dispose();
    }

#endif
}
