using System;
using System.Drawing;
using System.Windows.Forms;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    internal sealed class ModelLoadingDialog : Form
    {
        private readonly Label lblMessage;
        private readonly ProgressBar progressBar;

        public ModelLoadingDialog(string message)
        {
            Text = "Loading Model";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            ControlBox = false;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(380, 130);
            Padding = new Padding(18);

            lblMessage = new Label
            {
                Dock = DockStyle.Top,
                Height = 62,
                Text = message,
                TextAlign = ContentAlignment.MiddleLeft
            };

            progressBar = new ProgressBar
            {
                Dock = DockStyle.Top,
                Height = 22,
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 35
            };

            Controls.Add(progressBar);
            Controls.Add(lblMessage);

            UiTheme.Apply(this);
        }

        public static ModelLoadingDialog ShowFor(Control owner, string message)
        {
            var dialog = new ModelLoadingDialog(message);
            Form ownerForm = owner?.FindForm();

            if (ownerForm != null)
            {
                dialog.Location = GetCenteredLocation(ownerForm.Bounds, dialog.Size, Screen.FromControl(ownerForm).WorkingArea);
                dialog.Show(ownerForm);
            }
            else
            {
                Rectangle workingArea = Screen.PrimaryScreen?.WorkingArea ?? SystemInformation.WorkingArea;
                dialog.Location = GetCenteredLocation(workingArea, dialog.Size, workingArea);
                dialog.Show();
            }

            dialog.BringToFront();
            dialog.Refresh();
            return dialog;
        }

        private static Point GetCenteredLocation(Rectangle ownerBounds, Size dialogSize, Rectangle workingArea)
        {
            int x = ownerBounds.Left + (ownerBounds.Width - dialogSize.Width) / 2;
            int y = ownerBounds.Top + (ownerBounds.Height - dialogSize.Height) / 2;

            x = Math.Max(workingArea.Left, Math.Min(x, workingArea.Right - dialogSize.Width));
            y = Math.Max(workingArea.Top, Math.Min(y, workingArea.Bottom - dialogSize.Height));

            return new Point(x, y);
        }

        public void CloseSafely()
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action(CloseSafely));
                return;
            }

            Close();
        }
    }
}
