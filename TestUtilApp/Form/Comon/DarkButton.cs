using System.Drawing;
using System.Windows.Forms;

namespace TestUtilApp.UI
{
    /// <summary>
    /// Flat button that renders ForeColor even when disabled (WinForms ignores ForeColor on disabled buttons by default).
    /// </summary>
    internal class DarkButton : Button
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!Enabled)
            {
                TextRenderer.DrawText(
                    e.Graphics, Text, Font, ClientRectangle, ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            }
        }
    }
}
