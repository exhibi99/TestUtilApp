using System;

namespace TestUtilApp.CameraLight
{
    public class StubLightController : ILightController
    {
        public bool IsConnected  { get; private set; }
        public int  ChannelCount => 4;

        public void Connect(ISerialPort port)             => IsConnected = true;
        public void Disconnect()                          => IsConnected = false;

        public void SetIntensity(int channel, int value)  { }
        public void SetPattern(int channel, LightPattern pattern) { }
        public void TurnOn(int channel)                   { }
        public void TurnOff(int channel)                  { }
        public void TurnOffAll()                          { }

        public void Dispose() => Disconnect();
    }
}
