using System;

namespace TestUtilApp.CameraLight
{
    /// <summary>
    /// ALT RS232 조명 컨트롤러 (ALT-E8RS-24V / ALT-E16RS-24V)
    ///
    /// 단채널 On  : 0x4C 0x12 ch val xor 0x0D 0x0A  (7 bytes, ch/val 0-based)
    /// 전채널 8ch : 0xEF 0xEF 0x00 v0..v7 xor 0xEE 0xEE  (14 bytes)
    /// 전채널 16ch: 0x4C 0x17 v0..v15 xor 0x0D 0x0A  (21 bytes)
    /// </summary>
    public class AltLightController : ILightController
    {
        private readonly AltModel _model;
        private readonly int[]    _intensities;   // 채널별 현재 밝기 (0-based)

        private ISerialPort _port;

        public bool IsConnected  { get; private set; }
        public int  ChannelCount { get; }

        public AltLightController(AltModel model)
        {
            _model        = model;
            ChannelCount  = model == AltModel.E16RS ? 16 : 8;
            _intensities  = new int[ChannelCount];
        }

        public void Connect(ISerialPort port)
        {
            _port       = port ?? throw new ArgumentNullException(nameof(port));
            IsConnected = true;
        }

        public void Disconnect()
        {
            IsConnected = false;
            _port       = null;
        }

        // ── 공개 API ──────────────────────────────────────────────────

        public void SetIntensity(int channel, int value)
        {
            Validate(channel);
            value = Math.Max(0, Math.Min(255, value));
            _intensities[channel - 1] = value;
            SendSingleChannel(channel - 1, value);  // 단채널 패킷
        }

        public void TurnOn(int channel)
        {
            Validate(channel);
            int val = _intensities[channel - 1] > 0 ? _intensities[channel - 1] : 128;
            _intensities[channel - 1] = val;
            SendSingleChannel(channel - 1, val);
        }

        public void TurnOff(int channel)
        {
            Validate(channel);
            SendSingleChannel(channel - 1, 0);
        }

        public void TurnOffAll()
        {
            var zeros = new int[ChannelCount];
            SendAllChannels(zeros);
        }

        public void SetPattern(int channel, LightPattern pattern) { /* 미사용 */ }

        // ── 패킷 빌드 ────────────────────────────────────────────────

        // 단채널: 0x4C 0x12 ch val (0x12^ch^val) 0x0D 0x0A
        private void SendSingleChannel(int ch0, int val)
        {
            byte bCh  = (byte)ch0;
            byte bVal = (byte)val;
            byte[] msg =
            {
                0x4C, 0x12, bCh, bVal,
                (byte)(0x12 ^ bCh ^ bVal),
                0x0D, 0x0A
            };
            Write(msg);
        }

        private void SendAllChannels(int[] values)
        {
            if (_model == AltModel.E16RS)
                SendAllChannels16(values);
            else
                SendAllChannels8(values);
        }

        // 8ch 전채널: 0xEF 0xEF 0x00 v0..v7 xor 0xEE 0xEE  (14 bytes)
        private void SendAllChannels8(int[] values)
        {
            byte[] msg = new byte[14];
            msg[0] = 0xEF; msg[1] = 0xEF; msg[2] = 0x00;
            for (int i = 0; i < 8; i++) msg[3 + i] = (byte)values[i];
            // xor: msg[2..9] ^ (msg[10]+1)
            byte xor = msg[2];
            for (int i = 0; i < 7; i++) xor ^= msg[3 + i];
            xor ^= (byte)(msg[10] + 0x01);
            msg[11] = xor;
            msg[12] = 0xEE; msg[13] = 0xEE;
            Write(msg);
        }

        // 16ch 전채널: 0x4C 0x17 v0..v15 xor 0x0D 0x0A  (21 bytes)
        private void SendAllChannels16(int[] values)
        {
            byte[] msg = new byte[21];
            msg[0] = 0x4C; msg[1] = 0x17;
            for (int i = 0; i < 16; i++) msg[2 + i] = (byte)values[i];
            byte xor = msg[1];
            for (int i = 0; i < 16; i++) xor ^= msg[2 + i];
            msg[18] = xor;
            msg[19] = 0x0D; msg[20] = 0x0A;
            Write(msg);
        }

        private void Write(byte[] msg)
        {
            if (!IsConnected) throw new InvalidOperationException("Light controller is not connected.");
            _port.Write(msg);
        }

        private void Validate(int channel)
        {
            if (channel < 1 || channel > ChannelCount)
                throw new ArgumentOutOfRangeException(nameof(channel),
                    $"Channel must be 1–{ChannelCount}.");
        }

        public void Dispose() => Disconnect();
    }
}
