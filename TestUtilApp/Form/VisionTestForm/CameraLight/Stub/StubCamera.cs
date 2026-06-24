using System;
using System.Drawing;
using System.Threading;

namespace TestUtilApp.CameraLight
{
    public class StubCamera : ICamera
    {
        public bool   IsConnected    { get; private set; }
        public string ConnectedSerial { get; private set; } = "";

        private Timer           _liveTimer;
        private Action<Bitmap>  _liveCallback;
        private volatile bool   _liveStopped;
        private int             _frameIndex;

        public void Connect(string identifier)
        {
            // 빈 identifier는 stub_1 으로 확정
            ConnectedSerial = string.IsNullOrWhiteSpace(identifier) ? "stub_1" : identifier;
            IsConnected = true;
        }
        public void Disconnect()                  { StopLive(); IsConnected = false; ConnectedSerial = ""; }

        public void SetExposure(double microseconds) { }
        public void SetGain(double gain)             { }
        public void SetTriggerMode(TriggerMode mode) { }

        public (GrabResult result, Bitmap image) GrabSingle(int timeoutMs = 3000)
            => (GrabResult.Success, MakeFrame(_frameIndex++));

        public void StartLive(Action<Bitmap> onFrame)
        {
            if (_liveTimer != null) return;
            _liveStopped  = false;
            _liveCallback = onFrame;
            _liveTimer    = new Timer(_ =>
            {
                if (_liveStopped || _liveCallback == null) return;
                _liveCallback(MakeFrame(_frameIndex++));
            }, null, 0, 100);
        }

        public void StopLive()
        {
            _liveStopped = true;   // 타이머 콜백이 즉시 인식
            _liveTimer?.Dispose();
            _liveTimer    = null;
            _liveCallback = null;
        }

        private static Bitmap MakeFrame(int index)
        {
            var bmp = new Bitmap(640, 480);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(30, 30, 30));
                int bar = (index * 4) % 640;
                g.FillRectangle(Brushes.DimGray, bar, 0, 20, 480);
                using (var font = new Font("Segoe UI", 14f))
                using (var br   = new SolidBrush(Color.LimeGreen))
                    g.DrawString($"STUB  frame={index}", font, br, new PointF(20, 20));
            }
            return bmp;
        }

        public void Dispose() => Disconnect();
    }
}
