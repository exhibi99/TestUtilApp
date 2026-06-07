using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    public partial class AcquireDialog : Form
    {
        public string ImagePath { get; private set; }
        public bool LoadAsGray { get; private set; }

        public AcquireDialog()
        {
            InitializeComponent();
            UiTheme.Apply(this);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.tiff;*.tif|All Files|*.*";
                dlg.Title = "Select Image";

                if (!string.IsNullOrEmpty(txtImagePath.Text) && File.Exists(txtImagePath.Text))
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(txtImagePath.Text);
                }

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtImagePath.Text = dlg.FileName;
                    UpdatePreview();
                    UpdateOKButton();
                }
            }
        }

        private void radioColor_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (string.IsNullOrEmpty(txtImagePath.Text) || !File.Exists(txtImagePath.Text))
            {
                ClearPreview();
                return;
            }

            try
            {
                Image original = Image.FromFile(txtImagePath.Text);

                if (radioGray.Checked)
                {
                    Bitmap gray = ToGrayscale(original);
                    SetPreviewImage(gray);
                    original.Dispose();
                }
                else
                {
                    SetPreviewImage(original);
                }
            }
            catch
            {
                ClearPreview();
            }
        }

        private Bitmap ToGrayscale(Image source)
        {
            var bmp = new Bitmap(source.Width, source.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                var cm = new ColorMatrix(new float[][]
                {
                    new float[] { .299f, .299f, .299f, 0, 0 },
                    new float[] { .587f, .587f, .587f, 0, 0 },
                    new float[] { .114f, .114f, .114f, 0, 0 },
                    new float[] { 0,     0,     0,     1, 0 },
                    new float[] { 0,     0,     0,     0, 1 }
                });
                using (var attr = new ImageAttributes())
                {
                    attr.SetColorMatrix(cm);
                    g.DrawImage(source,
                        new Rectangle(0, 0, source.Width, source.Height),
                        0, 0, source.Width, source.Height,
                        GraphicsUnit.Pixel, attr);
                }
            }
            return bmp;
        }

        private void SetPreviewImage(Image img)
        {
            var old = pictureBoxPreview.Image;
            pictureBoxPreview.Image = img;
            old?.Dispose();
        }

        private void ClearPreview()
        {
            var old = pictureBoxPreview.Image;
            pictureBoxPreview.Image = null;
            old?.Dispose();
        }

        private void UpdateOKButton()
        {
            btnOK.Enabled = !string.IsNullOrEmpty(txtImagePath.Text) && File.Exists(txtImagePath.Text);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ImagePath = txtImagePath.Text;
            LoadAsGray = radioGray.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ClearPreview();
            base.OnFormClosed(e);
        }
    }
}
