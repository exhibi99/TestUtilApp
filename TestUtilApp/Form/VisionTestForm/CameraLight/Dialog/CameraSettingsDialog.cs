using System;
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

        public CameraSettingsDialog(CameraConfig config)
        {
            Config = config;
            InitializeComponent();
            EnumerateCameras();
            LoadValues();
        }

        private void EnumerateCameras()
        {
            string prev = Config.Identifier;
            cmbCamera.Items.Clear();

#if BASLER_PYLON
            try
            {
                var cameras = CameraFinder.Enumerate();
                string firstSn = cameras.Count > 0 ? cameras[0][CameraInfoKey.SerialNumber] : null;
                cmbCamera.Items.Add(firstSn != null
                    ? $"(첫번째 연결 카메라 ({firstSn}))"
                    : "(첫번째 카메라 - 미감지)");

                foreach (var info in cameras)
                {
                    string sn    = info[CameraInfoKey.SerialNumber];
                    string model = info[CameraInfoKey.ModelName];
                    cmbCamera.Items.Add($"{sn}  [{model}]");
                }
            }
            catch
            {
                cmbCamera.Items.Add("(첫번째 카메라 - 미감지)");
            }
#else
            cmbCamera.Items.Add("(첫번째 카메라)");
#endif

            int idx = 0;
            if (!string.IsNullOrWhiteSpace(prev))
            {
                for (int i = 1; i < cmbCamera.Items.Count; i++)
                {
                    if (cmbCamera.Items[i].ToString().StartsWith(prev))
                    {
                        idx = i;
                        break;
                    }
                }
            }
            cmbCamera.SelectedIndex = idx;
        }

        private void LoadValues()
        {
            numExposure.Value = (decimal)Config.ExposureUs;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            EnumerateCameras();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cmbCamera.SelectedIndex <= 0)
            {
                Config.Identifier = "";
            }
            else
            {
                string item = cmbCamera.SelectedItem?.ToString() ?? "";
                int sp = item.IndexOf(' ');
                Config.Identifier = sp > 0 ? item.Substring(0, sp) : item;
            }
            Config.ExposureUs = (double)numExposure.Value;
        }
    }
}
