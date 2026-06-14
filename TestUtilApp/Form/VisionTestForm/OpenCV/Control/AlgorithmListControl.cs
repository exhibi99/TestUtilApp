using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TestUtilApp.UI
{
    public partial class AlgorithmListControl : UserControl
    {
        private readonly List<IOpenCvAlgorithm> _algorithms = new List<IOpenCvAlgorithm>();
        private readonly List<Bitmap>            _previewBitmaps = new List<Bitmap>();
        private AlgorithmSettingsPanel           _currentSettingsPanel;
        private bool                             _updatingInputFrom;
        private PipelineViewForm                 _pipelineView;

        public event EventHandler AlgorithmListChanged;
        public event EventHandler RunRequested;
        public event EventHandler SettingsApplied;
        public event EventHandler SelectedAlgorithmChanged;

        public IReadOnlyList<IOpenCvAlgorithm> Algorithms => _algorithms.AsReadOnly();

        public int SelectedIndex =>
            listView.SelectedIndices.Count > 0 ? listView.SelectedIndices[0] : -1;

        public IOpenCvAlgorithm SelectedAlgorithm =>
            SelectedIndex >= 0 && SelectedIndex < _algorithms.Count
                ? _algorithms[SelectedIndex] : null;

        /// <summary>Returns the preview bitmap at index (reference only — do not dispose).</summary>
        public Bitmap GetPreviewBitmapAt(int index) =>
            (index >= 0 && index < _previewBitmaps.Count) ? _previewBitmaps[index] : null;

        /// <summary>Reloads the current settings panel from the algorithm's current values.</summary>
        public void RefreshCurrentSettingsPanel()
        {
            int idx = SelectedIndex;
            if (idx >= 0 && idx < _algorithms.Count && _currentSettingsPanel != null)
                _currentSettingsPanel.LoadFrom(_algorithms[idx]);
        }

        public AlgorithmListControl()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposePreviewBitmaps();
                _currentSettingsPanel?.Dispose();
                _currentSettingsPanel = null;
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        // ── Public API ────────────────────────────────────────────

        public void AddAlgorithm(IOpenCvAlgorithm algorithm)
        {
            _algorithms.Add(algorithm);
            RefreshList();
            int newIdx = listView.Items.Count - 1;
            listView.Items[newIdx].Selected = true;
            listView.EnsureVisible(newIdx);
            RaiseListChanged();
        }

        public void Clear()
        {
            if (_algorithms.Count == 0) return;
            _algorithms.Clear();
            listView.Items.Clear();
            DisposePreviewBitmaps();
            SwapSettingsPanel(-1);
            UpdatePreview();
            UpdateButtons();
            RaiseListChanged();
            SelectedAlgorithmChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Receives per-step intermediate bitmaps produced by the pipeline.
        /// Ownership of the bitmaps is transferred to this control.
        /// </summary>
        public void SetPreviewBitmaps(IList<Bitmap> bitmaps)
        {
            // Clear the PictureBox before disposing the old list
            pictureBoxPreview.Image = null;
            DisposePreviewBitmaps();
            _previewBitmaps.AddRange(bitmaps);
            UpdatePreview();
        }

        // ── Internal operations ───────────────────────────────────

        private void MoveSelectedUp()
        {
            int idx = SelectedIndex;
            if (idx <= 0) return;

            var item = _algorithms[idx];
            _algorithms.RemoveAt(idx);
            _algorithms.Insert(idx - 1, item);
            // Reset input routing — step indices shift after a reorder
            ResetInputFromSteps();
            RefreshList();
            listView.Items[idx - 1].Selected = true;
            RaiseListChanged();
        }

        private void MoveSelectedDown()
        {
            int idx = SelectedIndex;
            if (idx < 0 || idx >= _algorithms.Count - 1) return;

            var item = _algorithms[idx];
            _algorithms.RemoveAt(idx);
            _algorithms.Insert(idx + 1, item);
            ResetInputFromSteps();
            RefreshList();
            listView.Items[idx + 1].Selected = true;
            RaiseListChanged();
        }

        private void ResetInputFromSteps()
        {
            foreach (var a in _algorithms)
                a.InputFromStep = -1;
        }

        private void RemoveSelected()
        {
            int idx = SelectedIndex;
            if (idx < 0 || idx >= _algorithms.Count) return;

            _algorithms.RemoveAt(idx);
            RefreshList();

            if (_algorithms.Count > 0)
                listView.Items[Math.Min(idx, _algorithms.Count - 1)].Selected = true;
            else
            {
                SwapSettingsPanel(-1);
                SelectedAlgorithmChanged?.Invoke(this, EventArgs.Empty);
            }

            RaiseListChanged();
        }

        // ── Settings panel ────────────────────────────────────────

        private void SwapSettingsPanel(int idx)
        {
            // Detach and dispose old panel
            if (_currentSettingsPanel != null)
            {
                _currentSettingsPanel.Applied -= OnSettingsPanelApplied;
                pnlSettings.Controls.Remove(_currentSettingsPanel);
                _currentSettingsPanel.Dispose();
                _currentSettingsPanel = null;
            }

            UpdateInputFromCombo(idx);

            if (idx < 0 || idx >= _algorithms.Count)
            {
                lblNoSelection.Visible = true;
                return;
            }

            var panel = _algorithms[idx].GetSettingsPanel();
            if (panel == null)
            {
                lblNoSelection.Visible = true;
                return;
            }

            panel.LoadFrom(_algorithms[idx]);
            panel.Applied += OnSettingsPanelApplied;
            panel.Dock     = DockStyle.Fill;
            pnlSettings.Controls.Add(panel);

            lblNoSelection.Visible = false;
            _currentSettingsPanel  = panel;
        }

        // ── Input-from combo ──────────────────────────────────────

        private void UpdateInputFromCombo(int idx)
        {
            _updatingInputFrom = true;
            try
            {
                cboInputFrom.Items.Clear();

                bool hide = idx < 0 || idx >= _algorithms.Count || _algorithms[idx].IsSourceNode || idx == 0;
                pnlInputFrom.Visible = !hide;
                if (hide) return;

                cboInputFrom.Items.Add("Auto (이전 단계)");
                for (int i = 0; i < idx; i++)
                    cboInputFrom.Items.Add($"Step {i + 1}  {_algorithms[i].Name}");

                int cur = _algorithms[idx].InputFromStep;
                cboInputFrom.SelectedIndex = (cur >= 0 && cur < idx) ? cur + 1 : 0;
            }
            finally
            {
                _updatingInputFrom = false;
            }
        }

        private void cboInputFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingInputFrom) return;
            int algoIdx = SelectedIndex;
            if (algoIdx <= 0 || algoIdx >= _algorithms.Count) return;

            int sel = cboInputFrom.SelectedIndex;
            _algorithms[algoIdx].InputFromStep = sel <= 0 ? -1 : sel - 1;
            SettingsApplied?.Invoke(this, EventArgs.Empty);
        }

        private void OnSettingsPanelApplied(object sender, EventArgs e)
        {
            int idx = SelectedIndex;
            if (idx < 0 || idx >= _algorithms.Count || _currentSettingsPanel == null) return;

            _currentSettingsPanel.ApplyTo(_algorithms[idx]);
            RefreshList();
            if (idx < listView.Items.Count)
                listView.Items[idx].Selected = true;

            SettingsApplied?.Invoke(this, EventArgs.Empty);
        }

        // ── Preview ───────────────────────────────────────────────

        private void UpdatePreview()
        {
            int idx = SelectedIndex;
            var bmp = (idx >= 0 && idx < _previewBitmaps.Count) ? _previewBitmaps[idx] : null;
            pictureBoxPreview.Image  = bmp;
            btnSavePreview.Enabled   = bmp != null;
            btnSavePreview.ForeColor = bmp != null
                ? System.Drawing.Color.FromArgb(100, 200, 255)
                : System.Drawing.Color.Gray;
        }

        private void btnSavePreview_Click(object sender, EventArgs e)
        {
            var bmp = pictureBoxPreview.Image as Bitmap;
            if (bmp == null) return;

            string defaultName = SelectedAlgorithm != null
                ? $"step{SelectedIndex + 1}_{SelectedAlgorithm.Name}"
                : "preview";

            using (var dlg = new SaveFileDialog())
            {
                dlg.Title      = "Save Preview Image";
                dlg.Filter     = "PNG Files|*.png|JPEG Files|*.jpg|BMP Files|*.bmp";
                dlg.DefaultExt = "png";
                dlg.FileName   = defaultName;

                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var format = System.Drawing.Imaging.ImageFormat.Png;
                    string ext = System.IO.Path.GetExtension(dlg.FileName).ToLowerInvariant();
                    if (ext == ".jpg" || ext == ".jpeg")
                        format = System.Drawing.Imaging.ImageFormat.Jpeg;
                    else if (ext == ".bmp")
                        format = System.Drawing.Imaging.ImageFormat.Bmp;

                    bmp.Save(dlg.FileName, format);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Save error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DisposePreviewBitmaps()
        {
            foreach (var b in _previewBitmaps) b?.Dispose();
            _previewBitmaps.Clear();
        }

        // ── UI helpers ────────────────────────────────────────────

        private void RefreshList()
        {
            listView.BeginUpdate();
            listView.Items.Clear();
            for (int i = 0; i < _algorithms.Count; i++)
            {
                var  algo    = _algorithms[i];
                bool enabled = algo.IsEnabled;
                var  item    = new ListViewItem($"{i + 1}.  {algo.Name}{(enabled ? "" : "  [OFF]")}");
                item.SubItems.Add(algo.Summary);
                item.ForeColor = enabled
                    ? listView.ForeColor
                    : System.Drawing.Color.FromArgb(100, 100, 110);
                listView.Items.Add(item);
            }
            listView.EndUpdate();
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            int  idx     = SelectedIndex;
            bool hasSel  = idx >= 0;
            bool canUp   = hasSel && idx > 0;
            bool canDown = hasSel && idx < _algorithms.Count - 1;

            btnMoveUp.Enabled   = canUp;
            btnMoveDown.Enabled = canDown;
            btnRemove.Enabled   = hasSel;
            btnRun.Enabled      = _algorithms.Count > 0;
        }

        private void RaiseListChanged()
        {
            AlgorithmListChanged?.Invoke(this, EventArgs.Empty);
            _pipelineView?.RefreshView();
        }

        // ── Event handlers ────────────────────────────────────────

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
            SwapSettingsPanel(SelectedIndex);
            UpdatePreview();
            SelectedAlgorithmChanged?.Invoke(this, EventArgs.Empty);
        }

        private void btnMoveUp_Click(object sender, EventArgs e)   => MoveSelectedUp();
        private void btnMoveDown_Click(object sender, EventArgs e) => MoveSelectedDown();
        private void btnRemove_Click(object sender, EventArgs e)   => RemoveSelected();

        private void btnRun_Click(object sender, EventArgs e)
        {
            RunRequested?.Invoke(this, EventArgs.Empty);
        }

        private void menuRemove_Click(object sender, EventArgs e)   => RemoveSelected();
        private void menuClearAll_Click(object sender, EventArgs e) => Clear();

        // ── Pipeline View popup ───────────────────────────────────

        private void btnPipelineView_Click(object sender, EventArgs e)
        {
            if (_pipelineView != null && !_pipelineView.IsDisposed)
            {
                _pipelineView.BringToFront();
                return;
            }

            _pipelineView = new PipelineViewForm(_algorithms);

            // Position the popup to the left of this control
            var screenPt = PointToScreen(Point.Empty);
            _pipelineView.Location = new Point(
                Math.Max(0, screenPt.X - _pipelineView.Width - 6),
                screenPt.Y);

            _pipelineView.ConnectionsChanged += (s2, e2) =>
            {
                RefreshList();
                _pipelineView.RefreshView();
                SettingsApplied?.Invoke(this, EventArgs.Empty);
            };

            _pipelineView.SettingsApplied += (s2, e2) =>
            {
                RefreshList();
                RefreshCurrentSettingsPanel();
                SettingsApplied?.Invoke(this, EventArgs.Empty);
            };

            _pipelineView.SelectionChanged += (s2, e2) =>
            {
                int idx = _pipelineView.SelectedIndex;
                if (idx >= 0 && idx < listView.Items.Count)
                {
                    listView.Items[idx].Selected = true;
                    listView.EnsureVisible(idx);
                }
                SwapSettingsPanel(idx);
                UpdateButtons();
                UpdatePreview();
                SelectedAlgorithmChanged?.Invoke(this, EventArgs.Empty);
            };

            _pipelineView.MoveRequested += (idx, dir) =>
            {
                int newIdx = idx + dir;
                if (newIdx < 0 || newIdx >= _algorithms.Count) return;
                var item = _algorithms[idx];
                _algorithms.RemoveAt(idx);
                _algorithms.Insert(newIdx, item);
                ResetInputFromSteps();
                RefreshList();
                listView.Items[newIdx].Selected = true;
                _pipelineView.RefreshView(newIdx);
                RaiseListChanged();
            };

            _pipelineView.AlgorithmAddRequested += algo =>
            {
                AddAlgorithm(algo);
                _pipelineView?.RefreshView(_algorithms.Count - 1);
                SettingsApplied?.Invoke(this, EventArgs.Empty);
            };
            _pipelineView.RemoveRequested       += (s2, e2) => RemoveSelected();
            _pipelineView.ClearAllRequested     += (s2, e2) => Clear();
            _pipelineView.FormClosed            += (s2, e2) => _pipelineView = null;

            var owner = TopLevelControl as Form ?? FindForm();
            _pipelineView.Show(owner);

            // Sync current selection into the popup
            if (SelectedIndex >= 0)
                _pipelineView.SetSelection(SelectedIndex);
        }
    }
}
