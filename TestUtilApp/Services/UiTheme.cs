using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TestUtilApp.Services
{
    public static class UiTheme
    {
        public static readonly Color Window = Color.FromArgb(12, 14, 18);
        public static readonly Color Surface = Color.FromArgb(22, 25, 31);
        public static readonly Color SurfaceAlt = Color.FromArgb(30, 34, 42);
        public static readonly Color NavySurface = Color.FromArgb(15, 28, 55);
        public static readonly Color NavyButton = Color.FromArgb(25, 42, 75);
        public static readonly Color SurfaceHover = Color.FromArgb(42, 47, 57);
        public static readonly Color Input = Color.FromArgb(15, 17, 22);
        public static readonly Color Border = Color.FromArgb(64, 71, 84);
        public static readonly Color TextPrimary = Color.FromArgb(128, 255, 255);
        public static readonly Color LogText = Color.White;
        public static readonly Color TextMuted = Color.FromArgb(160, 169, 181);
        public static readonly Color Accent = Color.FromArgb(0, 120, 215);
        public static readonly Color AccentHover = Color.FromArgb(22, 139, 236);
        public static readonly Color Success = Color.FromArgb(88, 199, 121);
        public static readonly Color Warning = Color.FromArgb(245, 186, 65);
        public static readonly Color Error = Color.FromArgb(245, 99, 99);

        private const int CcmFirst = 0x2000;
        private const int PbmSetBkColor = CcmFirst + 1;
        private const int PbmSetBarColor = 0x0400 + 9;

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public static void Apply(Control root)
        {
            if (root == null)
            {
                return;
            }

            root.SuspendLayout();
            ApplyControl(root);

            foreach (Control child in root.Controls)
            {
                Apply(child);
            }

            root.ResumeLayout(false);
        }

        public static void ApplyNavigationButtonDefault(Button button)
        {
            if (button == null)
            {
                return;
            }

            ApplyButton(button, false);
        }

        public static void ApplyNavigationButtonActive(Button button)
        {
            if (button == null)
            {
                return;
            }

            ApplyButton(button, true);
        }

        private static void ApplyControl(Control control)
        {
            Color originalBackColor = control.BackColor;
            control.ForeColor = TextPrimary;

            if (control is Form || control is UserControl)
            {
                control.BackColor = Window;
            }
            else if (control is Panel)
            {
                control.BackColor = control.Name == "topPanel" ? NavySurface : Surface;
            }
            else if (control is GroupBox)
            {
                control.BackColor = Surface;
            }
            else if (control is TabPage tabPage)
            {
                tabPage.BackColor = Surface;
                tabPage.UseVisualStyleBackColor = false;
            }
            else
            {
                control.BackColor = Surface;
            }

            if (control is Label label)
            {
                ApplyLabel(label);
            }
            else if (control is Button button)
            {
                ApplyButton(button, IsPrimaryButton(button, originalBackColor));
            }
            else if (control is TextBoxBase textBox)
            {
                ApplyTextBox(textBox);
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.BackColor = Input;
                comboBox.ForeColor = TextPrimary;
                comboBox.FlatStyle = FlatStyle.Flat;
            }
            else if (control is NumericUpDown numericUpDown)
            {
                numericUpDown.BackColor = Input;
                numericUpDown.ForeColor = TextPrimary;
            }
            else if (control is CheckBox checkBox)
            {
                checkBox.BackColor = Surface;
                checkBox.ForeColor = TextPrimary;
                checkBox.UseVisualStyleBackColor = false;
                checkBox.FlatStyle = FlatStyle.Flat;
            }
            else if (control is RadioButton radioButton)
            {
                radioButton.BackColor = Surface;
                radioButton.ForeColor = TextPrimary;
                radioButton.UseVisualStyleBackColor = false;
                radioButton.FlatStyle = FlatStyle.Flat;
            }
            else if (control is ListView listView)
            {
                ApplyListView(listView);
            }
            else if (control is ListBox listBox)
            {
                ApplyListBox(listBox);
            }
            else if (control is ProgressBar progressBar)
            {
                ApplyProgressBar(progressBar);
            }
            else if (control is DataGridView dataGridView)
            {
                ApplyDataGridView(dataGridView);
            }
            else if (control is PropertyGrid propertyGrid)
            {
                ApplyPropertyGrid(propertyGrid);
            }
            else if (control is PictureBox pictureBox)
            {
                pictureBox.BackColor = Color.Black;
            }
            else if (control is TabControl tabControl)
            {
                ApplyTabControl(tabControl);
            }

            if (control is ToolStrip toolStrip)
            {
                ApplyToolStrip(toolStrip);
            }
        }

        private static void ApplyLabel(Label label)
        {
            if (label.ForeColor.ToArgb() == Color.Gray.ToArgb())
            {
                label.ForeColor = TextMuted;
            }
            else
            {
                label.ForeColor = TextPrimary;
            }

            label.BackColor = Color.Transparent;
        }

        private static bool IsPrimaryButton(Button button, Color originalBackColor)
        {
            return originalBackColor.ToArgb() == Accent.ToArgb()
                || button.Name.StartsWith("btnStart", StringComparison.OrdinalIgnoreCase)
                || string.Equals(button.Text, "Label Generate", StringComparison.OrdinalIgnoreCase)
                || string.Equals(button.Text, "Start Filtering", StringComparison.OrdinalIgnoreCase)
                || button.Text.StartsWith("Start ", StringComparison.OrdinalIgnoreCase)
                || string.Equals(button.Text, "Save", StringComparison.OrdinalIgnoreCase);
        }

        private static void ApplyButton(Button button, bool isPrimary)
        {
            button.UseVisualStyleBackColor = false;
            button.FlatStyle = FlatStyle.Flat;
            button.ForeColor = TextPrimary;
            bool isNavyPanel = button.Parent?.Name == "topPanel";
            button.BackColor = isPrimary ? Accent : (isNavyPanel ? NavyButton : SurfaceAlt);
            button.FlatAppearance.BorderColor = isPrimary ? Accent : Border;
            button.FlatAppearance.MouseOverBackColor = isPrimary ? AccentHover : SurfaceHover;
            button.FlatAppearance.MouseDownBackColor = isPrimary ? Accent : Color.FromArgb(49, 55, 66);
        }

        private static void ApplyTextBox(TextBoxBase textBox)
        {
            textBox.BackColor = Input;
            textBox.ForeColor = LogText;
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void ApplyListView(ListView listView)
        {
            listView.BackColor = Input;
            listView.ForeColor = TextPrimary;
            listView.BorderStyle = BorderStyle.FixedSingle;
            listView.GridLines = true;
            listView.FullRowSelect = true;
            listView.HideSelection = false;
            listView.OwnerDraw = true;

            listView.DrawColumnHeader -= DrawListViewColumnHeader;
            listView.DrawColumnHeader += DrawListViewColumnHeader;
            listView.DrawItem -= DrawListViewItem;
            listView.DrawItem += DrawListViewItem;
            listView.DrawSubItem -= DrawListViewSubItem;
            listView.DrawSubItem += DrawListViewSubItem;
        }

        private static void DrawListViewColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (var backBrush = new SolidBrush(SurfaceAlt))
            using (var borderPen = new Pen(Border))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
                e.Graphics.DrawRectangle(borderPen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
            }

            Rectangle textBounds = e.Bounds;
            textBounds.Inflate(-6, 0);
            TextRenderer.DrawText(
                e.Graphics,
                e.Header.Text,
                e.Font,
                textBounds,
                TextPrimary,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static void DrawListViewItem(object sender, DrawListViewItemEventArgs e)
        {
            var listView = sender as ListView;
            if (listView == null || listView.View == View.Details)
            {
                return;
            }

            bool selected = e.Item.Selected;
            Color backColor = selected ? Accent : (e.ItemIndex % 2 == 0 ? Input : Window);
            Color textColor = selected ? Color.White : GetItemTextColor(e.Item.ForeColor);

            using (var backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            Rectangle textBounds = e.Bounds;
            textBounds.Inflate(-6, 0);
            TextRenderer.DrawText(
                e.Graphics,
                e.Item.Text,
                listView.Font,
                textBounds,
                textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static void DrawListViewSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            var listView = sender as ListView;
            if (listView == null)
            {
                return;
            }

            bool selected = e.Item.Selected;
            Color backColor = selected ? Accent : (e.ItemIndex % 2 == 0 ? Input : Window);
            Color textColor = selected ? Color.White : GetItemTextColor(e.SubItem.ForeColor);

            using (var backBrush = new SolidBrush(backColor))
            using (var borderPen = new Pen(Border))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
                e.Graphics.DrawRectangle(borderPen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
            }

            Rectangle textBounds = e.Bounds;
            textBounds.Inflate(-6, 0);
            TextRenderer.DrawText(
                e.Graphics,
                e.SubItem.Text,
                listView.Font,
                textBounds,
                textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static Color GetItemTextColor(Color color)
        {
            if (color.IsEmpty || color.ToArgb() == Color.Black.ToArgb() || color.ToArgb() == SystemColors.WindowText.ToArgb())
            {
                return TextPrimary;
            }

            return color;
        }

        private static void ApplyListBox(ListBox listBox)
        {
            listBox.BackColor = Input;
            listBox.ForeColor = TextPrimary;
            listBox.BorderStyle = BorderStyle.FixedSingle;
            listBox.DrawMode = DrawMode.OwnerDrawFixed;
            listBox.DrawItem -= DrawListBoxItem;
            listBox.DrawItem += DrawListBoxItem;
        }

        private static void DrawListBoxItem(object sender, DrawItemEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null || e.Index < 0 || e.Index >= listBox.Items.Count)
            {
                return;
            }

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color backColor = selected ? Accent : (e.Index % 2 == 0 ? Input : Window);
            Color textColor = selected ? Color.White : TextPrimary;

            using (var backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            Rectangle textBounds = e.Bounds;
            textBounds.Inflate(-6, 0);
            TextRenderer.DrawText(
                e.Graphics,
                listBox.Items[e.Index].ToString(),
                e.Font,
                textBounds,
                textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static void ApplyProgressBar(ProgressBar progressBar)
        {
            progressBar.BackColor = Input;
            progressBar.ForeColor = Accent;
            progressBar.HandleCreated -= ProgressBar_HandleCreated;
            progressBar.HandleCreated += ProgressBar_HandleCreated;
            ApplyProgressBarNativeColors(progressBar);
        }

        private static void ProgressBar_HandleCreated(object sender, EventArgs e)
        {
            ApplyProgressBarNativeColors(sender as ProgressBar);
        }

        private static void ApplyProgressBarNativeColors(ProgressBar progressBar)
        {
            if (progressBar == null || !progressBar.IsHandleCreated)
            {
                return;
            }

            SetWindowTheme(progressBar.Handle, "", "");
            SendMessage(progressBar.Handle, PbmSetBkColor, IntPtr.Zero, ToColorRef(Input));
            SendMessage(progressBar.Handle, PbmSetBarColor, IntPtr.Zero, ToColorRef(Accent));
        }

        private static IntPtr ToColorRef(Color color)
        {
            return (IntPtr)(color.R | (color.G << 8) | (color.B << 16));
        }

        private static void ApplyTabControl(TabControl tabControl)
        {
            tabControl.BackColor = Window;
            tabControl.Appearance = TabAppearance.Normal;
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem -= DrawTabItem;
            tabControl.DrawItem += DrawTabItem;

            foreach (TabPage page in tabControl.TabPages)
            {
                page.BackColor = Surface;
                page.ForeColor = TextPrimary;
                page.UseVisualStyleBackColor = false;
            }
        }

        private static void DrawTabItem(object sender, DrawItemEventArgs e)
        {
            var tabControl = sender as TabControl;
            if (tabControl == null || e.Index < 0 || e.Index >= tabControl.TabPages.Count)
            {
                return;
            }

            bool selected = e.Index == tabControl.SelectedIndex;
            Rectangle bounds = e.Bounds;
            Color backColor = selected ? Accent : SurfaceAlt;
            Color textColor = selected ? Color.White : TextMuted;

            if (e.Index == 0)
            {
                PaintTabControlBackground(tabControl, e.Graphics);
            }

            using (var backBrush = new SolidBrush(backColor))
            using (var borderPen = new Pen(selected ? Accent : Border))
            {
                bounds.Inflate(-1, -1);
                e.Graphics.FillRectangle(backBrush, bounds);
                e.Graphics.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }

            TextRenderer.DrawText(
                e.Graphics,
                tabControl.TabPages[e.Index].Text,
                tabControl.Font,
                bounds,
                textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static void PaintTabControlBackground(TabControl tabControl, Graphics graphics)
        {
            Rectangle pageBounds = tabControl.DisplayRectangle;
            pageBounds.Inflate(2, 2);

            int headerHeight = Math.Max(tabControl.ItemSize.Height + 8, tabControl.DisplayRectangle.Top);
            Rectangle headerBounds = new Rectangle(0, 0, tabControl.Width, headerHeight);

            using (var headerBrush = new SolidBrush(Window))
            using (var pageBrush = new SolidBrush(Surface))
            using (var borderPen = new Pen(Border))
            using (var headerRegion = new Region(headerBounds))
            {
                for (int i = 0; i < tabControl.TabPages.Count; i++)
                {
                    headerRegion.Exclude(tabControl.GetTabRect(i));
                }

                graphics.FillRegion(headerBrush, headerRegion);
                graphics.FillRectangle(pageBrush, pageBounds);
                graphics.DrawRectangle(borderPen, pageBounds.X, pageBounds.Y, pageBounds.Width - 1, pageBounds.Height - 1);
            }
        }

        private static void ApplyDataGridView(DataGridView dataGridView)
        {
            dataGridView.BackgroundColor = Window;
            dataGridView.BorderStyle = BorderStyle.FixedSingle;
            dataGridView.GridColor = Border;
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.DefaultCellStyle.BackColor = Input;
            dataGridView.DefaultCellStyle.ForeColor = TextPrimary;
            dataGridView.DefaultCellStyle.SelectionBackColor = Accent;
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = SurfaceAlt;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
            dataGridView.RowHeadersDefaultCellStyle.BackColor = SurfaceAlt;
            dataGridView.RowHeadersDefaultCellStyle.ForeColor = TextPrimary;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Surface;
            dataGridView.AlternatingRowsDefaultCellStyle.ForeColor = TextPrimary;
        }

        private static void ApplyPropertyGrid(PropertyGrid propertyGrid)
        {
            propertyGrid.BackColor = Surface;
            propertyGrid.ForeColor = TextPrimary;
            propertyGrid.ViewBackColor = Input;
            propertyGrid.ViewForeColor = TextPrimary;
            propertyGrid.ViewBorderColor = Border;
            propertyGrid.LineColor = Border;
            propertyGrid.HelpBackColor = Surface;
            propertyGrid.HelpForeColor = TextMuted;
            propertyGrid.CommandsBackColor = Surface;
            propertyGrid.CommandsForeColor = TextPrimary;
            propertyGrid.CategoryForeColor = TextPrimary;
            propertyGrid.CategorySplitterColor = Border;
        }

        private static void ApplyToolStrip(ToolStrip toolStrip)
        {
            toolStrip.BackColor = SurfaceAlt;
            toolStrip.ForeColor = TextPrimary;
            toolStrip.Renderer = new ToolStripProfessionalRenderer(new DarkColorTable());

            foreach (ToolStripItem item in toolStrip.Items)
            {
                item.BackColor = SurfaceAlt;
                item.ForeColor = TextMuted;
            }
        }

        private sealed class DarkColorTable : ProfessionalColorTable
        {
            public override Color ToolStripGradientBegin => SurfaceAlt;
            public override Color ToolStripGradientMiddle => SurfaceAlt;
            public override Color ToolStripGradientEnd => SurfaceAlt;
            public override Color StatusStripGradientBegin => SurfaceAlt;
            public override Color StatusStripGradientEnd => SurfaceAlt;
            public override Color MenuBorder => Border;
            public override Color ButtonSelectedBorder => Border;
            public override Color ButtonSelectedGradientBegin => SurfaceHover;
            public override Color ButtonSelectedGradientMiddle => SurfaceHover;
            public override Color ButtonSelectedGradientEnd => SurfaceHover;
            public override Color ButtonPressedGradientBegin => Accent;
            public override Color ButtonPressedGradientMiddle => Accent;
            public override Color ButtonPressedGradientEnd => Accent;
        }
    }
}
