using System;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TestUtilApp.CameraLight
{
    public class ComPortSettingsDialog : Form
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            int value = 1;
            DwmSetWindowAttribute(Handle, 20, ref value, sizeof(int));
        }

        private ComboBox cmbPort;
        private ComboBox cmbBaud;
        private Button   btnOk;
        private Button   btnCancel;

        public ComPortConfig Config { get; }

        public ComPortSettingsDialog(ComPortConfig config)
        {
            Config = config;
            BuildUI();
            LoadValues();
        }

        private void BuildUI()
        {
            Text            = "COM Port Settings";
            Size            = new System.Drawing.Size(280, 160);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition   = FormStartPosition.CenterParent;
            MaximizeBox     = false;
            MinimizeBox     = false;

            var c0   = System.Drawing.Color.FromArgb(18, 22, 35);
            var c1   = System.Drawing.Color.FromArgb(25, 42, 75);
            var cCyan= System.Drawing.Color.FromArgb(128, 255, 255);
            var cGray= System.Drawing.Color.FromArgb(160, 169, 181);
            BackColor = c0;

            int y = 16;
            Add(new Label { Text = "Port:", Location = new System.Drawing.Point(12, y), AutoSize = true, ForeColor = cGray });
            cmbPort = new ComboBox
            {
                Location      = new System.Drawing.Point(80, y - 2),
                Size          = new System.Drawing.Size(90, 26),
                BackColor     = c1, ForeColor = cCyan,
                DropDownStyle = ComboBoxStyle.DropDownList,
            };
            cmbPort.Items.AddRange(SerialPort.GetPortNames());
            Add(cmbPort);
            y += 36;

            Add(new Label { Text = "Baud Rate:", Location = new System.Drawing.Point(12, y), AutoSize = true, ForeColor = cGray });
            cmbBaud = new ComboBox
            {
                Location      = new System.Drawing.Point(80, y - 2),
                Size          = new System.Drawing.Size(90, 26),
                BackColor     = c1, ForeColor = cCyan,
                DropDownStyle = ComboBoxStyle.DropDownList,
            };
            cmbBaud.Items.AddRange(new object[] { 9600, 19200, 38400, 57600, 115200 });
            Add(cmbBaud);
            y += 48;

            btnOk = new Button { Text = "OK", Location = new System.Drawing.Point(80, y), Size = new System.Drawing.Size(72, 28), BackColor = c1, ForeColor = cCyan, FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancel", Location = new System.Drawing.Point(162, y), Size = new System.Drawing.Size(72, 28), BackColor = c1, ForeColor = cGray, FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.Cancel };
            Add(btnOk); Add(btnCancel);
            AcceptButton = btnOk;
            CancelButton = btnCancel;
            btnOk.Click += (_, __) => SaveValues();
        }

        private void LoadValues()
        {
            int portIdx = cmbPort.Items.IndexOf(Config.PortName);
            cmbPort.SelectedIndex = portIdx >= 0 ? portIdx : (cmbPort.Items.Count > 0 ? 0 : -1);

            int baudIdx = cmbBaud.Items.IndexOf(Config.BaudRate);
            cmbBaud.SelectedIndex = baudIdx >= 0 ? baudIdx : 0;
        }

        private void SaveValues()
        {
            if (cmbPort.SelectedItem != null) Config.PortName = cmbPort.SelectedItem.ToString();
            if (cmbBaud.SelectedItem != null) Config.BaudRate = (int)cmbBaud.SelectedItem;
        }

        private void Add(Control c) => Controls.Add(c);
    }
}
