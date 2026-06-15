using System;
using System.IO.Ports;

namespace TestUtilApp.CameraLight
{
    public class WindowsSerialPort : ISerialPort
    {
        private SerialPort _port;

        public bool IsOpen => _port?.IsOpen ?? false;

        public void Open(string portName, int baudRate)
        {
            Close();
            _port = new SerialPort(portName, baudRate)
            {
                DataBits    = 8,
                Parity      = Parity.None,
                StopBits    = StopBits.One,
                ReadTimeout  = 500,
                WriteTimeout = 500,
            };
            _port.Open();
        }

        public void Close()
        {
            if (_port == null) return;
            if (_port.IsOpen) _port.Close();
            _port.Dispose();
            _port = null;
        }

        public void Write(byte[] data)
        {
            if (!IsOpen) throw new InvalidOperationException("Serial port is not open.");
            _port.Write(data, 0, data.Length);
        }

        public byte[] ReadResponse(int expectedLength, int timeoutMs = 500)
        {
            if (!IsOpen) throw new InvalidOperationException("Serial port is not open.");

            _port.ReadTimeout = timeoutMs;
            var buf = new byte[expectedLength];
            int received = 0;
            while (received < expectedLength)
            {
                int n = _port.Read(buf, received, expectedLength - received);
                if (n == 0) break;
                received += n;
            }
            return buf;
        }

        public void Dispose() => Close();
    }
}
