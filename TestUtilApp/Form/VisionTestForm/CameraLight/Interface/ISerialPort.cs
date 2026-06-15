using System;

namespace TestUtilApp.CameraLight
{
    public interface ISerialPort : IDisposable
    {
        bool IsOpen { get; }

        void Open(string portName, int baudRate);
        void Close();

        void Write(byte[] data);
        byte[] ReadResponse(int expectedLength, int timeoutMs = 500);
    }
}
