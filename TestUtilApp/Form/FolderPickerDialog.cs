using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    // ── Public API ────────────────────────────────────────────────────────────
    public static class FolderPickerDialog
    {
        public static string Show(IWin32Window owner, string title, string initialPath = null)
        {
            using (var dlg = new FolderPickerForm(title, initialPath))
            {
                var result = owner != null ? dlg.ShowDialog(owner) : dlg.ShowDialog();
                return result == DialogResult.OK ? dlg.SelectedPath : null;
            }
        }
    }

    // ── Form implementation ───────────────────────────────────────────────────
    internal sealed class FolderPickerForm : Form
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

        public string SelectedPath { get; private set; }

        private TextBox  _txtPath;
        private TreeView _tree;
        private Button   _btnOk;
        private TreeNode _rightClickedNode;
        private const string Placeholder = "\x01";

        // ── Construction ──────────────────────────────────────────────────────
        public FolderPickerForm(string title, string initialPath)
        {
            BuildLayout(title);
            PopulateRoots();

            if (!string.IsNullOrWhiteSpace(initialPath) && Directory.Exists(initialPath))
                NavigateTo(initialPath);
        }

        private void BuildLayout(string title)
        {
            Text            = title;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition   = FormStartPosition.CenterParent;
            ShowInTaskbar   = false;
            BackColor       = UiTheme.Window;
            Size            = new Size(580, 500);
            MinimumSize     = new Size(420, 360);
            KeyPreview      = true;

            // ── accent strip ─────────────────────────────────────────────
            var strip = new Panel { Dock = DockStyle.Left, Width = 4, BackColor = UiTheme.TealAccent };

            // ── title bar ────────────────────────────────────────────────
            var titlePanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 44,
                BackColor = UiTheme.Surface,
                Cursor    = Cursors.SizeAll,
            };
            AttachDragMove(titlePanel);
            var lblTitle = new Label
            {
                Text      = title,
                Font      = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                AutoSize  = true,
                Location  = new Point(16, 13),
            };
            var btnX = MakeBtn("✕", false);
            btnX.Size     = new Size(32, 28);
            btnX.Font     = new Font("Segoe UI", 9f);
            btnX.ForeColor = UiTheme.TextMuted;
            btnX.FlatAppearance.BorderSize = 0;
            btnX.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            titlePanel.Controls.Add(lblTitle);
            titlePanel.Controls.Add(btnX);
            titlePanel.Resize += (s, e) => btnX.Location = new Point(titlePanel.Width - 36, 8);

            // ── path row ─────────────────────────────────────────────────
            var pathPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 38,
                BackColor = UiTheme.SurfaceAlt,
            };
            var lblPath = new Label
            {
                Text      = "Path:",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = UiTheme.TextMuted,
                AutoSize  = true,
                Location  = new Point(12, 11),
            };
            _txtPath = new TextBox
            {
                Font        = new Font("Consolas", 9.5f),
                BackColor   = UiTheme.Input,
                ForeColor   = UiTheme.LogText,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor      = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
            };
            _txtPath.KeyDown += TxtPath_KeyDown;
            pathPanel.Controls.Add(lblPath);
            pathPanel.Controls.Add(_txtPath);
            pathPanel.Resize += (s, e) => _txtPath.SetBounds(52, 7, pathPanel.Width - 64, 24);

            // ── button panel ─────────────────────────────────────────────
            var btnPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 52,
                BackColor = UiTheme.Surface,
            };
            var btnNewFolder = MakeBtn("New Folder", false);
            var btnCancel    = MakeBtn("Cancel", false);
            _btnOk           = MakeBtn("OK", true);
            btnNewFolder.Click += BtnNewFolder_Click;
            btnCancel.Click    += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            _btnOk.Click       += BtnOk_Click;
            btnPanel.Controls.AddRange(new Control[] { btnNewFolder, btnCancel, _btnOk });
            btnPanel.Resize += (s, e) =>
            {
                _btnOk.Location       = new Point(btnPanel.Width - 110, 9);
                btnCancel.Location    = new Point(btnPanel.Width - 220, 9);
                btnNewFolder.Location = new Point(12, 9);
            };

            var separator = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = UiTheme.Border };

            // ── tree view ────────────────────────────────────────────────
            _tree = new BufferedTreeView
            {
                Dock          = DockStyle.Fill,
                BackColor     = UiTheme.Input,
                ForeColor     = UiTheme.TextPrimary,
                Font          = new Font("Segoe UI", 9.5f),
                BorderStyle   = BorderStyle.None,
                HideSelection = false,
                HotTracking   = false,
                ShowLines     = false,
                ShowPlusMinus = true,
                FullRowSelect = true,
                ItemHeight    = 26,
                DrawMode      = TreeViewDrawMode.OwnerDrawAll,
                Indent        = 20,
            };
            // TreeView는 DoubleBuffered 프로퍼티가 protected — 리플렉션으로 활성화
            typeof(TreeView)
                .GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(_tree, true);

            _tree.HandleCreated        += (s, e) => SetWindowTheme(_tree.Handle, "DarkMode_Explorer", null);
            _tree.BeforeExpand         += Tree_BeforeExpand;
            _tree.AfterSelect          += Tree_AfterSelect;
            _tree.DrawNode             += Tree_DrawNode;
            _tree.NodeMouseClick       += Tree_NodeMouseClick;
            _tree.ContextMenuStrip      = BuildContextMenu();

            // ── assemble ─────────────────────────────────────────────────
            Controls.Add(_tree);
            Controls.Add(separator);
            Controls.Add(btnPanel);
            Controls.Add(pathPanel);
            Controls.Add(titlePanel);
            Controls.Add(strip);

            AcceptButton = _btnOk;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape) { DialogResult = DialogResult.Cancel; Close(); }
                if (e.KeyCode == Keys.F5)     RefreshNode(_tree.SelectedNode);
            };
        }

        // ── Context menu ──────────────────────────────────────────────────────
        private ContextMenuStrip BuildContextMenu()
        {
            var menu = new ContextMenuStrip();
            menu.Renderer  = new DarkMenuRenderer();
            menu.BackColor = UiTheme.SurfaceAlt;
            menu.ForeColor = UiTheme.TextPrimary;
            menu.Font      = new Font("Segoe UI", 9.5f);

            var itemRename = new ToolStripMenuItem("Rename") { ForeColor = UiTheme.TextPrimary };
            var itemDelete = new ToolStripMenuItem("Delete") { ForeColor = UiTheme.Error };

            itemRename.Click += (s, e) => RenameNode(_rightClickedNode);
            itemDelete.Click += (s, e) => DeleteNode(_rightClickedNode);

            menu.Items.Add(itemRename);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(itemDelete);

            menu.Opening += (s, e) =>
            {
                bool valid = _rightClickedNode?.Tag is string p && Directory.Exists(p);
                itemRename.Enabled = valid;
                itemDelete.Enabled = valid;
            };

            return menu;
        }

        private void Tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            _tree.SelectedNode  = e.Node;
            _rightClickedNode   = e.Node;
        }

        private void RenameNode(TreeNode node)
        {
            if (node == null) return;
            string oldPath = node.Tag as string;
            if (!Directory.Exists(oldPath)) return;

            string oldName = Path.GetFileName(oldPath.TrimEnd('\\', '/'));
            string newName = PromptFolderName("Rename Folder", oldName);
            if (string.IsNullOrWhiteSpace(newName) || newName == oldName) return;

            string parent  = Path.GetDirectoryName(oldPath);
            string newPath = Path.Combine(parent, newName);
            try
            {
                Directory.Move(oldPath, newPath);
                node.Text = newName;
                node.Tag  = newPath;
                SetPathBox(newPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to rename: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteNode(TreeNode node)
        {
            if (node == null) return;
            string path = node.Tag as string;
            if (!Directory.Exists(path)) return;

            string name = Path.GetFileName(path.TrimEnd('\\', '/'));
            var result = ThemedDialog.Show(this,
                "Delete Folder",
                $"Delete '{name}' and all its contents?",
                ThemedDialog.DialogButtons.YesNo,
                ThemedDialog.DialogIcon.Warning,
                new Size(480, 220));

            if (result != DialogResult.Yes) return;
            try
            {
                Directory.Delete(path, recursive: true);
                node.Remove();
                SetPathBox(string.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Flicker-free TreeView ─────────────────────────────────────────────
        private sealed class BufferedTreeView : TreeView
        {
            private const int WM_ERASEBKGND = 0x0014;

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_ERASEBKGND)
                {
                    m.Result = IntPtr.Zero;  // 배경 지우기 억제 → 깜빡임 제거
                    return;
                }
                base.WndProc(ref m);
            }
        }

        // ── Dark menu renderer ────────────────────────────────────────────────
        private sealed class DarkMenuRenderer : ToolStripProfessionalRenderer
        {
            public DarkMenuRenderer() : base(new DarkMenuColors()) { }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = e.Item.Enabled ? (Color)e.Item.ForeColor : UiTheme.TextMuted;
                base.OnRenderItemText(e);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                var bounds = new Rectangle(4, e.Item.Height / 2, e.Item.Width - 8, 1);
                using (var pen = new Pen(UiTheme.Border))
                    e.Graphics.DrawLine(pen, bounds.Left, bounds.Y, bounds.Right, bounds.Y);
            }
        }

        private sealed class DarkMenuColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected          => UiTheme.SurfaceHover;
            public override Color MenuItemSelectedGradientBegin => UiTheme.SurfaceHover;
            public override Color MenuItemSelectedGradientEnd   => UiTheme.SurfaceHover;
            public override Color MenuItemBorder           => UiTheme.Border;
            public override Color MenuBorder               => UiTheme.Border;
            public override Color ToolStripDropDownBackground => UiTheme.SurfaceAlt;
            public override Color ImageMarginGradientBegin => UiTheme.SurfaceAlt;
            public override Color ImageMarginGradientMiddle => UiTheme.SurfaceAlt;
            public override Color ImageMarginGradientEnd   => UiTheme.SurfaceAlt;
        }

        // ── Drag-to-move ──────────────────────────────────────────────────────
        private void AttachDragMove(Control handle)
        {
            Point dragStart = Point.Empty;
            bool  dragging  = false;

            handle.MouseDown += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                dragging  = true;
                dragStart = e.Location;
            };
            handle.MouseMove += (s, e) =>
            {
                if (!dragging) return;
                Location = new Point(
                    Location.X + e.X - dragStart.X,
                    Location.Y + e.Y - dragStart.Y);
            };
            handle.MouseUp += (s, e) => dragging = false;
        }

        // ── Controls factory ──────────────────────────────────────────────────
        private static Button MakeBtn(string text, bool primary)
        {
            var btn = new Button
            {
                Text      = text,
                Size      = new Size(100, 34),
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor    = Cursors.Hand,
            };
            if (primary)
            {
                btn.BackColor = UiTheme.TealAccent;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderColor        = UiTheme.TealAccent;
                btn.FlatAppearance.MouseOverBackColor = UiTheme.TealAccentHover;
                btn.FlatAppearance.MouseDownBackColor = UiTheme.TealAccentDown;
            }
            else
            {
                btn.BackColor = UiTheme.SurfaceAlt;
                btn.ForeColor = UiTheme.TextPrimary;
                btn.FlatAppearance.BorderColor        = UiTheme.Border;
                btn.FlatAppearance.MouseOverBackColor = UiTheme.SurfaceHover;
            }
            return btn;
        }

        // ── TreeView drawing (OwnerDrawAll) ──────────────────────────────────
        private void Tree_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Bounds.Width <= 0 || e.Bounds.Height <= 0) return;

            var g        = e.Graphics;
            bool selected = (e.State & TreeNodeStates.Selected) != 0;
            bool focused  = _tree.Focused && selected;

            Color back = selected ? (focused ? UiTheme.Accent : UiTheme.SurfaceHover) : UiTheme.Input;
            Color fore = selected ? Color.White : UiTheme.TextPrimary;

            // ── full-row background ──────────────────────────────────────
            var row = new Rectangle(0, e.Bounds.Y, _tree.ClientSize.Width, e.Bounds.Height);
            using (var br = new SolidBrush(back))
                g.FillRectangle(br, row);

            // ── expand / collapse glyph ──────────────────────────────────
            if (e.Node.Nodes.Count > 0)
            {
                int glyphSize = 9;
                int cx = e.Node.Level * _tree.Indent + 10;
                int cy = e.Bounds.Y + e.Bounds.Height / 2;
                var glyphRect = new Rectangle(cx - glyphSize / 2, cy - glyphSize / 2, glyphSize, glyphSize);

                using (var borderPen = new Pen(UiTheme.Border))
                    g.DrawRectangle(borderPen, glyphRect);

                using (var glyphBrush = new SolidBrush(UiTheme.Input))
                    g.FillRectangle(glyphBrush, glyphRect.X + 1, glyphRect.Y + 1,
                        glyphRect.Width - 1, glyphRect.Height - 1);

                using (var linePen = new Pen(UiTheme.TextMuted, 1.5f))
                {
                    // horizontal line (always)
                    g.DrawLine(linePen,
                        glyphRect.X + 2, cy,
                        glyphRect.Right - 2, cy);
                    // vertical line (only when collapsed)
                    if (!e.Node.IsExpanded)
                        g.DrawLine(linePen,
                            cx, glyphRect.Y + 2,
                            cx, glyphRect.Bottom - 2);
                }
            }

            // ── folder icon + text ───────────────────────────────────────
            int textX = e.Node.Level * _tree.Indent + 24;
            string text = "📁  " + e.Node.Text;
            var textBounds = new Rectangle(textX, e.Bounds.Y,
                _tree.ClientSize.Width - textX - 4, e.Bounds.Height);
            TextRenderer.DrawText(g, text, e.Node.NodeFont ?? _tree.Font,
                textBounds, fore,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

            e.DrawDefault = false;
        }

        // ── Root population ───────────────────────────────────────────────────
        private void PopulateRoots()
        {
            _tree.BeginUpdate();
            _tree.Nodes.Clear();
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady) continue;
                string root = drive.RootDirectory.FullName;
                var node = new TreeNode(root) { Tag = root };
                _tree.Nodes.Add(node);
                PopulateChildren(node);  // 1차 하위 폴더 바로 로드
                node.Expand();
            }
            _tree.EndUpdate();
        }

        private void PopulateChildren(TreeNode parent)
        {
            string path = parent.Tag as string;
            if (string.IsNullOrEmpty(path)) return;
            try
            {
                _tree.BeginUpdate();
                foreach (var dir in new DirectoryInfo(path).GetDirectories())
                {
                    if ((dir.Attributes & (FileAttributes.Hidden | FileAttributes.System)) != 0)
                        continue;
                    var child = new TreeNode(dir.Name) { Tag = dir.FullName };
                    if (HasSubFolders(dir.FullName))
                        child.Nodes.Add(new TreeNode(Placeholder));
                    parent.Nodes.Add(child);
                }
            }
            catch { }
            finally { _tree.EndUpdate(); }
        }

        private static bool HasSubFolders(string path)
        {
            try
            {
                using (var e = Directory.EnumerateDirectories(path).GetEnumerator())
                    return e.MoveNext();
            }
            catch { return false; }
        }

        // ── Navigation ────────────────────────────────────────────────────────
        private void NavigateTo(string targetPath)
        {
            if (!Directory.Exists(targetPath)) return;

            // Build ancestors list from root → target
            var parts = new System.Collections.Generic.List<string>();
            var di = new DirectoryInfo(targetPath);
            while (di != null)
            {
                parts.Insert(0, di.FullName);
                di = di.Parent;
            }

            TreeNodeCollection level = _tree.Nodes;
            TreeNode found = null;

            foreach (string part in parts)
            {
                found = null;
                foreach (TreeNode n in level)
                {
                    string nPath = (n.Tag as string) ?? n.Text;
                    if (string.Equals(nPath.TrimEnd('\\'), part.TrimEnd('\\'),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        found = n;
                        break;
                    }
                }
                if (found == null) break;

                // Lazy-load children if needed
                if (found.Nodes.Count == 1 && found.Nodes[0].Text == Placeholder)
                {
                    found.Nodes.Clear();
                    PopulateChildren(found);
                }
                found.Expand();
                level = found.Nodes;
            }

            if (found != null)
            {
                _tree.SelectedNode = found;
                found.EnsureVisible();
            }
            SetPathBox(targetPath);
        }

        private void RefreshNode(TreeNode node)
        {
            if (node == null) { PopulateRoots(); return; }
            string path = node.Tag as string;
            node.Nodes.Clear();
            if (path != null && HasSubFolders(path))
                node.Nodes.Add(new TreeNode(Placeholder));
            node.Expand();
        }

        // ── Event handlers ────────────────────────────────────────────────────
        private void Tree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var node = e.Node;
            if (node.Nodes.Count == 1 && node.Nodes[0].Text == Placeholder)
            {
                node.Nodes.Clear();
                PopulateChildren(node);
            }
        }

        private void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SetPathBox((e.Node.Tag as string) ?? e.Node.Text);
        }


        private void TxtPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            string path = _txtPath.Text.Trim();
            if (Directory.Exists(path))
                NavigateTo(path);
            e.Handled = e.SuppressKeyPress = true;
        }

        private void SetPathBox(string path)
        {
            _txtPath.Text = path;
        }

        // ── Button actions ────────────────────────────────────────────────────
        private void BtnOk_Click(object sender, EventArgs e)
        {
            string typed = _txtPath.Text.Trim();
            if (Directory.Exists(typed))
            {
                SelectedPath = typed;
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
            var node = _tree.SelectedNode;
            if (node == null) return;
            string path = (node.Tag as string) ?? node.Text;
            if (!Directory.Exists(path)) return;
            SelectedPath = path;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnNewFolder_Click(object sender, EventArgs e)
        {
            var node = _tree.SelectedNode;
            if (node == null) return;
            string parentPath = (node.Tag as string) ?? node.Text;
            if (!Directory.Exists(parentPath)) return;

            string name = PromptNewFolderName();
            if (string.IsNullOrWhiteSpace(name)) return;

            string newPath = Path.Combine(parentPath, name);
            try
            {
                Directory.CreateDirectory(newPath);
                if (node.Nodes.Count == 1 && node.Nodes[0].Text == Placeholder)
                {
                    node.Nodes.Clear();
                    PopulateChildren(node);
                }
                else
                {
                    var newNode = new TreeNode(name) { Tag = newPath };
                    node.Nodes.Add(newNode);
                }
                node.Expand();
                foreach (TreeNode child in node.Nodes)
                {
                    if (string.Equals(child.Tag as string, newPath, StringComparison.OrdinalIgnoreCase))
                    {
                        _tree.SelectedNode = child;
                        child.EnsureVisible();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"폴더를 만들 수 없습니다: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string PromptFolderName(string dialogTitle, string defaultText)
            => PromptNewFolderName(dialogTitle, defaultText);

        private string PromptNewFolderName()
            => PromptNewFolderName("New Folder", "New Folder");

        private string PromptNewFolderName(string dialogTitle, string defaultText)
        {
            using (var form = new Form())
            {
                form.FormBorderStyle = FormBorderStyle.None;
                form.StartPosition   = FormStartPosition.CenterParent;
                form.ShowInTaskbar   = false;
                form.BackColor       = UiTheme.Window;
                form.Size            = new Size(360, 160);
                form.KeyPreview      = true;

                // accent strip
                var strip = new Panel { Dock = DockStyle.Left, Width = 4, BackColor = UiTheme.TealAccent };

                // title bar
                var titlePanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = UiTheme.Surface, Cursor = Cursors.SizeAll };
                var lblTitle   = new Label
                {
                    Text      = dialogTitle,
                    Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                    ForeColor = UiTheme.TextPrimary,
                    AutoSize  = true,
                    Location  = new Point(14, 11),
                };
                titlePanel.Controls.Add(lblTitle);

                // drag-to-move on title bar
                Point dragStart = Point.Empty;
                bool  dragging  = false;
                titlePanel.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { dragging = true; dragStart = e.Location; } };
                titlePanel.MouseMove += (s, e) => { if (dragging) form.Location = new Point(form.Location.X + e.X - dragStart.X, form.Location.Y + e.Y - dragStart.Y); };
                titlePanel.MouseUp   += (s, e) => dragging = false;

                // body
                var body = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Window, Padding = new Padding(14, 10, 14, 0) };
                var lbl  = new Label
                {
                    Text      = "Folder name:",
                    Font      = new Font("Segoe UI", 9f),
                    ForeColor = UiTheme.TextMuted,
                    AutoSize  = true,
                    Location  = new Point(14, 10),
                };
                var txt = new TextBox
                {
                    Font        = new Font("Segoe UI", 9.5f),
                    BackColor   = UiTheme.Input,
                    ForeColor   = UiTheme.LogText,
                    BorderStyle = BorderStyle.FixedSingle,
                    Text        = defaultText,
                    Location    = new Point(14, 30),
                    Anchor      = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                };
                body.Controls.Add(lbl);
                body.Controls.Add(txt);
                body.Resize += (s, e) => txt.Width = body.ClientSize.Width - 28;

                // button panel
                var btnPanel  = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = UiTheme.Surface };
                var separator = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = UiTheme.Border };
                var btnOk     = MakeBtn("OK",     true);
                var btnCancel = MakeBtn("Cancel", false);
                btnOk.Click     += (s, e) => { form.DialogResult = DialogResult.OK;     form.Close(); };
                btnCancel.Click += (s, e) => { form.DialogResult = DialogResult.Cancel; form.Close(); };
                btnPanel.Controls.AddRange(new Control[] { btnOk, btnCancel });
                btnPanel.Resize += (s, e) =>
                {
                    btnOk.Location     = new Point(btnPanel.Width - 110, 8);
                    btnCancel.Location = new Point(btnPanel.Width - 220, 8);
                };

                form.Controls.Add(body);
                form.Controls.Add(separator);
                form.Controls.Add(btnPanel);
                form.Controls.Add(titlePanel);
                form.Controls.Add(strip);

                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;
                form.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Escape) { form.DialogResult = DialogResult.Cancel; form.Close(); }
                };
                form.Shown += (s, e) => { txt.SelectAll(); txt.Focus(); };

                return form.ShowDialog(this) == DialogResult.OK ? txt.Text.Trim() : null;
            }
        }
    }
}
