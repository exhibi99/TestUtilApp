using System;

namespace TestUtilApp.CameraLight
{
    public enum LightPattern { Continuous, Strobe }

    public interface ILightController : IDisposable
    {
        bool IsConnected { get; }
        int ChannelCount { get; }

        void Connect(ISerialPort port);
        void Disconnect();

        void SetIntensity(int channel, int value);   // 0–255
        void SetPattern(int channel, LightPattern pattern);
        void TurnOn(int channel);
        void TurnOff(int channel);
        void TurnOffAll();
    }
}
