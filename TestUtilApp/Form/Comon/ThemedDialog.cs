using System;
using System.Drawing;
using System.Windows.Forms;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    public sealed class ThemedDialog : Form
    {
        public enum DialogIcon { None, Info, Warning, Error, Question }
        public enum DialogButtons { OK, YesNo, OKCancel }

        private static readonly Color IconInfo     = Color.FromArgb(0, 120, 215);
        private static readonly Color IconWarning  = Color.FromArgb(245, 186, 65);
        private static readonly Color IconError    = Color.FromArgb(245, 99, 99);
        private static readonly Color IconQuestion = Color.FromArgb(0, 140, 120);

        private ThemedDialog() { }

        // ----------------------------------------------------------------
        // Public API
        // ----------------------------------------------------------------

        public static DialogResult Show(
            IWin32Window owner,
            string title,
            string content,
            DialogButtons buttons = DialogButtons.OK,
            DialogIcon icon = DialogIcon.Info,
            Size? size = null)
        {
            using (var dlg = Build(title, content, buttons, icon, size))
            {
                return owner != null ? dlg.ShowDialog(owner) : dlg.ShowDialog();
            }
        }

        // ----------------------------------------------------------------
        // Builder
        // ----------------------------------------------------------------

        private static ThemedDialog Build(
            string title, string content,
            DialogButtons buttons, DialogIcon icon, Size? size = null)
        {
            Color accentColor = GetIconColor(icon);
            string iconGlyph  = GetIconGlyph(icon);

            var dlg = new ThemedDialog
            {
                Text              = title,
                FormBorderStyle   = FormBorderStyle.None,
                StartPosition     = FormStartPosition.CenterParent,
                ShowInTaskbar     = false,
                BackColor         = UiTheme.Window,
                Size              = size ?? new Size(560, 440),
                Padding           = new Padding(0),
                MinimizeBox       = false,
                MaximizeBox       = false,
            };

            // ── accent strip (left edge) ──────────────────────────────
            var strip = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 4,
                BackColor = accentColor,
            };

            // ── title panel ──────────────────────────────────────────
            var titlePanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 52,
                BackColor = UiTheme.Surface,
                Padding   = new Padding(18, 0, 12, 0),
            };

            var lblIcon = new Label
            {
                Text      = iconGlyph,
                Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = accentColor,
                AutoSize  = true,
                Location  = new Point(16, 14),
            };

            var lblTitle = new Label
            {
                Text      = title,
                Font      = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                AutoSize  = true,
                Location  = new Point(44, 15),
            };

            titlePanel.Controls.Add(lblIcon);
            titlePanel.Controls.Add(lblTitle);

            // ── button panel ─────────────────────────────────────────
            var buttonPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 58,
                BackColor = UiTheme.Surface,
                Padding   = new Padding(0, 10, 16, 10),
            };

            AddButtons(dlg, buttonPanel, buttons, accentColor);

            // ── separator ────────────────────────────────────────────
            var separator = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 1,
                BackColor = UiTheme.Border,
            };

            // ── content area ─────────────────────────────────────────
            var contentBox = new RichTextBox
            {
                Dock          = DockStyle.Fill,
                ReadOnly      = true,
                BorderStyle   = BorderStyle.None,
                BackColor     = UiTheme.Window,
                ForeColor     = UiTheme.TextPrimary,
                Font          = new Font("Segoe UI", 10F),
                ScrollBars    = RichTextBoxScrollBars.Vertical,
                Margin        = new Padding(0),
                Padding       = new Padding(0),
                Text          = content,
                TabStop       = false,
            };

            var contentPad = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = UiTheme.Window,
                Padding   = new Padding(20, 16, 20, 8),
            };
            contentBox.Dock = DockStyle.Fill;
            contentPad.Controls.Add(contentBox);

            // ── assemble (reverse order for Dock) ────────────────────
            dlg.Controls.Add(contentPad);
            dlg.Controls.Add(separator);
            dlg.Controls.Add(buttonPanel);
            dlg.Controls.Add(titlePanel);
            dlg.Controls.Add(strip);

            // Esc key closes as cancel
            dlg.KeyPreview = true;
            dlg.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    dlg.DialogResult = DialogResult.Cancel;
                    dlg.Close();
                }
            };

            return dlg;
        }

        // ── button factory ───────────────────────────────────────────────
        private static void AddButtons(
            Form dlg, Panel panel,
            DialogButtons buttons, Color accentColor)
        {
            const int btnW = 100;
            const int btnH = 36;
            const int gap  = 10;

            Button MakeButton(string text, DialogResult result, bool isPrimary)
            {
                var btn = new Button
                {
                    Text      = text,
                    Size      = new Size(btnW, btnH),
                    FlatStyle = FlatStyle.Flat,
                    Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    Anchor    = AnchorStyles.Right | AnchorStyles.Top,
                    Cursor    = Cursors.Hand,
                };

                if (isPrimary)
                {
                    btn.BackColor = accentColor;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.BorderColor = accentColor;
                }
                else
                {
                    btn.BackColor = UiTheme.SurfaceAlt;
                    btn.ForeColor = UiTheme.TextPrimary;
                    btn.FlatAppearance.BorderColor = UiTheme.Border;
                }

                btn.Click += (s, e) => { dlg.DialogResult = result; dlg.Close(); };
                return btn;
            }

            switch (buttons)
            {
                case DialogButtons.YesNo:
                {
                    var btnNo  = MakeButton("No",  DialogResult.No,  false);
                    var btnYes = MakeButton("Yes", DialogResult.Yes, true);
                    btnNo.Location  = new Point(panel.Width - btnW * 2 - gap * 2, 10);
                    btnYes.Location = new Point(panel.Width - btnW - gap, 10);
                    btnNo.Anchor  = AnchorStyles.Right | AnchorStyles.Top;
                    btnYes.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                    panel.Controls.Add(btnNo);
                    panel.Controls.Add(btnYes);
                    dlg.AcceptButton = btnYes;
                    dlg.CancelButton = btnNo;
                    break;
                }
                case DialogButtons.OKCancel:
                {
                    var btnCancel = MakeButton("Cancel", DialogResult.Cancel, false);
                    var btnOk     = MakeButton("OK",     DialogResult.OK,     true);
                    btnCancel.Location = new Point(panel.Width - btnW * 2 - gap * 2, 10);
                    btnOk.Location     = new Point(panel.Width - btnW - gap, 10);
                    btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                    btnOk.Anchor     = AnchorStyles.Right | AnchorStyles.Top;
                    panel.Controls.Add(btnCancel);
                    panel.Controls.Add(btnOk);
                    dlg.AcceptButton = btnOk;
                    dlg.CancelButton = btnCancel;
                    break;
                }
                default: // OK
                {
                    var btnOk = MakeButton("OK", DialogResult.OK, true);
                    btnOk.Location = new Point(panel.Width - btnW - gap, 10);
                    btnOk.Anchor   = AnchorStyles.Right | AnchorStyles.Top;
                    panel.Controls.Add(btnOk);
                    dlg.AcceptButton = btnOk;
                    break;
                }
            }
        }

        // ── helpers ──────────────────────────────────────────────────────
        private static Color GetIconColor(DialogIcon icon)
        {
            switch (icon)
            {
                case DialogIcon.Warning:  return IconWarning;
                case DialogIcon.Error:    return IconError;
                case DialogIcon.Question: return IconQuestion;
                default:                  return IconInfo;
            }
        }

        private static string GetIconGlyph(DialogIcon icon)
        {
            switch (icon)
            {
                case DialogIcon.Warning:  return "⚠";
                case DialogIcon.Error:    return "✕";
                case DialogIcon.Question: return "?";
                default:                  return "ℹ";
            }
        }
    }
}
