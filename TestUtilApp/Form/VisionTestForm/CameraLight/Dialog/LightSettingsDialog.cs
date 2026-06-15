using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TestUtilApp.CameraLight
{
    public partial class LightSettingsDialog : Form
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            int value = 1;
            DwmSetWindowAttribute(Handle, 20, ref value, sizeof(int));
        }

        private readonly ILightController  _ctrl;
        private readonly LightConfig       _cfg;
        private readonly HashSet<int>      _selected = new HashSet<int>(); // 1-based

        // ── 채널 버튼 색상 ─────────────────────────────────────────────
        private static readonly Color CChBg      = Color.FromArgb(30, 33, 48);
        private static readonly Color CChBdr     = Color.FromArgb(55, 60, 78);
        private static readonly Color CChSelBg   = Color.FromArgb(20, 60, 95);
        private static readonly Color CChSelBdr  = Color.FromArgb(42, 148, 255);
        private static readonly Color CChText    = Color.FromArgb(160, 169, 181);
        private static readonly Color CChSelText = Color.FromArgb(128, 255, 255);

        public LightSettingsDialog(LightConfig config, ILightController ctrl = null)
        {
            _cfg  = config;
            _ctrl = ctrl;
            InitializeComponent();

            btnOn.FlatAppearance.BorderColor  = Color.FromArgb(50, 160, 100);
            btnOff.FlatAppearance.BorderColor = Color.FromArgb(100, 105, 115);

            cmbModel.SelectedIndex = _cfg.Model == AltModel.E16RS ? 1 : 0;
            RebuildChannelButtons();
        }

        // ── 모델 변경 ────────────────────────────────────────────────

        private void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            _cfg.Model = cmbModel.SelectedIndex == 1 ? AltModel.E16RS : AltModel.E8RS;
            _selected.Clear();
            RebuildChannelButtons();
        }

        // ── 채널 토글 버튼 빌드 ───────────────────────────────────────

        private void RebuildChannelButtons()
        {
            pnlChannels.Controls.Clear();

            int ch   = _cfg.ChannelCount;   // 8 or 16
            int cols = 4;                   // 항상 4열 (8ch: 4×2, 16ch: 4×4)
            int rows = (ch + cols - 1) / cols;
            int btnW = 60;
            int btnH = 28;
            int gap  = 4;

            // 다이얼로그 너비 고정 (Model 콤보·버튼 3개 기준)
            const int clientW = 330;
            int panelW = cols * btnW + (cols - 1) * gap;  // 4*60+3*4 = 252

            pnlChannels.Width  = panelW;
            pnlChannels.Height = rows * btnH + (rows - 1) * gap;
            pnlChannels.Left   = (clientW - panelW) / 2;

            // Intensity 행
            int baseY = pnlChannels.Bottom + 12;
            lblIntensity.Top     = baseY + 8;
            trkIntensity.Top     = baseY;
            trkIntensity.Left    = lblIntensity.Right + 4;
            trkIntensity.Width   = clientW - trkIntensity.Left - 46;  // 우측 40px = 값 레이블
            lblIntensityVal.Left = trkIntensity.Right + 4;
            lblIntensityVal.Top  = baseY + 8;

            // 버튼 3개 오른쪽 정렬
            btnClose.Left = clientW - 12 - btnClose.Width;
            btnOff.Left   = btnClose.Left - 10 - btnOff.Width;
            btnOn.Left    = btnOff.Left   - 10 - btnOn.Width;
            btnOn.Top     = trkIntensity.Bottom + 8;
            btnOff.Top    = btnOn.Top;
            btnClose.Top  = btnOn.Top;
            ClientSize    = new System.Drawing.Size(clientW, btnOn.Bottom + 12);

            for (int i = 0; i < ch; i++)
            {
                int channel = i + 1;
                int col = i % cols;
                int row = i / cols;

                var btn = new Button
                {
                    Text      = $"Ch {channel}",
                    Location  = new Point(col * (btnW + gap), row * (btnH + gap)),
                    Size      = new Size(btnW, btnH),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = CChBg,
                    ForeColor = CChText,
                    Font      = new Font("Segoe UI", 8f),
                    Tag       = channel,
                };
                btn.FlatAppearance.BorderColor = CChBdr;
                btn.Click += ChBtn_Click;
                pnlChannels.Controls.Add(btn);
            }
        }

        private void ChBtn_Click(object sender, EventArgs e)
        {
            var btn     = (Button)sender;
            int channel = (int)btn.Tag;

            if (_selected.Contains(channel))
            {
                _selected.Remove(channel);
                btn.BackColor                  = CChBg;
                btn.ForeColor                  = CChText;
                btn.FlatAppearance.BorderColor = CChBdr;
            }
            else
            {
                _selected.Add(channel);
                btn.BackColor                  = CChSelBg;
                btn.ForeColor                  = CChSelText;
                btn.FlatAppearance.BorderColor = CChSelBdr;
            }
        }

        // ── Intensity ────────────────────────────────────────────────

        private void trkIntensity_Scroll(object sender, EventArgs e)
        {
            lblIntensityVal.Text = trkIntensity.Value.ToString();
            if (_ctrl?.IsConnected == true)
                foreach (int ch in _selected)
                    _ctrl.SetIntensity(ch, trkIntensity.Value);
        }

        // ── ON / OFF ─────────────────────────────────────────────────

        private void btnOn_Click(object sender, EventArgs e)
        {
            if (_ctrl?.IsConnected != true) return;
            foreach (int ch in _selected)
                _ctrl.TurnOn(ch);
        }

        private void btnOff_Click(object sender, EventArgs e)
        {
            if (_ctrl?.IsConnected != true) return;
            foreach (int ch in _selected)
                _ctrl.TurnOff(ch);
        }
    }
}
