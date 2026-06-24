using System;
using System.Drawing;

namespace TestUtilApp.CameraLight
{
    public enum TriggerMode { FreeRun, Software, Hardware }
    public enum GrabResult { Success, Timeout, Error }

    public interface ICamera : IDisposable
    {
        bool IsConnected { get; }
        /// <summary>연결 후 실제 카메라 시리얼 번호. 연결 전에는 empty string.</summary>
        string ConnectedSerial { get; }

        void Connect(string identifier);
        void Disconnect();

        void SetExposure(double microseconds);
        void SetGain(double gain);
        void SetTriggerMode(TriggerMode mode);

        // 단발 촬영 — 결과 이미지 반환
        (GrabResult result, Bitmap image) GrabSingle(int timeoutMs = 3000);

        // 라이브 스트리밍 — 프레임마다 콜백 호출
        void StartLive(Action<Bitmap> onFrame);
        void StopLive();
    }
}
