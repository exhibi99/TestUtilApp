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

        public event EventHandler AlgorithmListChanged;
        public event EventHandler RunRequested;
        public event EventHandler SettingsApplied;

        public IReadOnlyList<IOpenCvAlgorithm> Algorithms => _algorithms.AsReadOnly();

        public int SelectedIndex =>
            listView.SelectedIndices.Count > 0 ? listView.SelectedIndices[0] : -1;

        public AlgorithmListControl()
        {
            InitializeComponent();
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
            RefreshList();
            listView.Items[idx + 1].Selected = true;
            RaiseListChanged();
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
                SwapSettingsPanel(-1);

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
            pictureBoxPreview.Image = (idx >= 0 && idx < _previewBitmaps.Count)
                ? _previewBitmaps[idx]
                : null;
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
                var item = new ListViewItem(_algorithms[i].Name);
                item.SubItems.Add(_algorithms[i].Summary);
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
        }

        // ── Event handlers ────────────────────────────────────────

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
            SwapSettingsPanel(SelectedIndex);
            UpdatePreview();
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
    }
}
