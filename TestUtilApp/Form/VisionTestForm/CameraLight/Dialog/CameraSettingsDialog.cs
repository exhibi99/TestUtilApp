using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#if BASLER_PYLON
using Basler.Pylon;
#endif

namespace TestUtilApp.CameraLight
{
    public partial class CameraSettingsDialog : Form
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            int value = 1;
            DwmSetWindowAttribute(Handle, 20, ref value, sizeof(int));
        }

        public CameraConfig Config { get; }

        // 콤보 아이템 내부 데이터: (serial, model)
        private readonly List<(string serial, string model)> _cameraList
            = new List<(string, string)>();

        public CameraSettingsDialog(CameraConfig config)
        {
            Config = config;
            InitializeComponent();
            EnumerateCameras();
            LoadValues();
        }

        // ── 카메라 열거 ───────────────────────────────────────────────

        private void EnumerateCameras()
        {
            _cameraList.Clear();
            cmbCamera.Items.Clear();

#if BASLER_PYLON
            try
            {
                foreach (var info in CameraFinder.Enumerate())
                {
                    string sn    = info[CameraInfoKey.SerialNumber];
                    string model = info[CameraInfoKey.ModelName];
                    _cameraList.Add((sn, model));
                    cmbCamera.Items.Add($"{model}  ({sn})");
                }
            }
            catch { }
#else
            // Stub 모드: 가상 카메라 2개 표시
            _cameraList.Add(("stub_1", "StubCamera"));
            _cameraList.Add(("stub_2", "StubCamera"));
            foreach (var (sn, model) in _cameraList)
                cmbCamera.Items.Add($"{model}  ({sn})");
#endif

            if (_cameraList.Count == 0)
            {
                UpdateSelectedLabel(-1);
                btnOk.Enabled = false;
                return;
            }

            btnOk.Enabled = true;

            // 이전에 선택된 카메라 복원, 없으면 첫 번째 선택
            int idx = 0;
            if (!string.IsNullOrWhiteSpace(Config.Identifier))
            {
                for (int i = 0; i < _cameraList.Count; i++)
                {
                    if (_cameraList[i].serial == Config.Identifier)
                    { idx = i; break; }
                }
            }
            cmbCamera.SelectedIndex = idx;
        }

        private void LoadValues()
        {
            numExposure.Value = (decimal)Math.Max((double)numExposure.Minimum,
                                   Math.Min((double)numExposure.Maximum, Config.ExposureUs));
        }

        // ── 이벤트 ───────────────────────────────────────────────────

        private void cmbCamera_SelectedIndexChanged(object sender, EventArgs e)
            => UpdateSelectedLabel(cmbCamera.SelectedIndex);

        private void UpdateSelectedLabel(int idx)
        {
            if (idx < 0 || idx >= _cameraList.Count)
            {
                lblSelected.Text = _cameraList.Count == 0
                    ? "연결된 카메라가 없습니다."
                    : "";
                return;
            }
            var (sn, model) = _cameraList[idx];
            lblSelected.Text = $"Selected Camera:  {model}  /  S/N: {sn}";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
            => EnumerateCameras();

        private void btnOk_Click(object sender, EventArgs e)
        {
            int idx = cmbCamera.SelectedIndex;
            Config.Identifier = (idx >= 0 && idx < _cameraList.Count)
                ? _cameraList[idx].serial
                : "";
            Config.ExposureUs = (double)numExposure.Value;
        }

        // ── 카메라 열거 유틸 (CameraLightControl 에서도 사용) ──────────

        public static List<(string serial, string model)> EnumeratePhysicalCameras()
        {
            var result = new List<(string, string)>();
#if BASLER_PYLON
            try
            {
                foreach (var info in CameraFinder.Enumerate())
                    result.Add((info[CameraInfoKey.SerialNumber], info[CameraInfoKey.ModelName]));
            }
            catch { }
#else
            result.Add(("stub_1", "StubCamera"));
            result.Add(("stub_2", "StubCamera"));
#endif
            return result;
        }
    }
}
