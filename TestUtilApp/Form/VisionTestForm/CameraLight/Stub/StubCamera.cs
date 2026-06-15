using System;
using System.Drawing;

namespace TestUtilApp.CameraLight
{
    public class StubCamera : ICamera
    {
        public bool IsConnected { get; private set; }

        public void Connect(string identifier)    => IsConnected = true;
        public void Disconnect()                  => IsConnected = false;

        public void SetExposure(double microseconds) { }
        public void SetGain(double gain)             { }
        public void SetTriggerMode(TriggerMode mode) { }

        public (GrabResult result, Bitmap image) GrabSingle(int timeoutMs = 3000)
        {
            var bmp = new Bitmap(640, 480);
            using (var g = Graphics.FromImage(bmp))
                g.Clear(Color.DimGray);
            return (GrabResult.Success, bmp);
        }

        public void StartLive(Action<Bitmap> onFrame) { }
        public void StopLive()                        { }

        public void Dispose() => Disconnect();
    }
}
