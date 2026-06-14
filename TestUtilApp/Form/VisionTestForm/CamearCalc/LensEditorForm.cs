using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TestUtilApp.UI
{
    public partial class LensEditorForm : Form
    {
        private static string ConfigPath  => CameraCalcControl.LensConfigJsonPath;
        private static string RuntimePath => CameraCalcControl.RuntimeLensJsonPath;

        public LensEditorForm(IEnumerable<CameraCalcControl.LensEntry> lenses)
        {
            InitializeComponent();
            ApplyTheme();
            LoadRows(lenses);
        }

        private void LoadRows(IEnumerable<CameraCalcControl.LensEntry> lenses)
        {
            dgvLenses.Rows.Clear();
            foreach (var l in lenses)
                dgvLenses.Rows.Add(l.name, l.manufacturer, l.focalLength, l.minWD, l.imageCircle);
        }

        // ─────────────────────────────────────────────
        //  Button handlers
        // ─────────────────────────────────────────────
        private void btnAdd_Click(object sender, EventArgs e)
        {
            int idx = dgvLenses.Rows.Add("New Lens", "Kowa", 25, 150, 11);
            dgvLenses.ClearSelection();
            dgvLenses.Rows[idx].Selected = true;
            dgvLenses.CurrentCell = dgvLenses.Rows[idx].Cells[0];
            dgvLenses.BeginEdit(true);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvLenses.SelectedRows)
                if (!row.IsNewRow) dgvLenses.Rows.Remove(row);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var list = BuildList();
            if (list == null) return;

            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            try
            {
                File.WriteAllText(RuntimePath, json);

                string srcConfig = CameraCalcControl.SourceLensConfigJsonPath;
                if (Directory.Exists(Path.GetDirectoryName(srcConfig)))
                    File.WriteAllText(srcConfig, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("저장 실패:\n" + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // ─────────────────────────────────────────────
        //  Validation & build
        // ─────────────────────────────────────────────
        private List<CameraCalcControl.LensEntry> BuildList()
        {
            var list = new List<CameraCalcControl.LensEntry>();
            for (int i = 0; i < dgvLenses.Rows.Count; i++)
            {
                var row = dgvLenses.Rows[i];
                if (row.IsNewRow) continue;

                string name = row.Cells[0].Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(name)) { ShowRowError(i, "렌즈명을 입력하세요."); return null; }

                string manufacturer = row.Cells[1].Value?.ToString()?.Trim() ?? "";

                if (!TryParseCell(row, 2, out double fl))          { ShowRowError(i, "초점거리 값이 올바르지 않습니다."); return null; }
                if (!TryParseCell(row, 3, out double minWD))       { ShowRowError(i, "최소WD 값이 올바르지 않습니다.");   return null; }
                if (!TryParseCell(row, 4, out double imageCircle)) { ShowRowError(i, "이미지서클 값이 올바르지 않습니다."); return null; }

                list.Add(new CameraCalcControl.LensEntry
                {
                    name = name, manufacturer = manufacturer,
                    focalLength = fl, minWD = minWD, imageCircle = imageCircle,
                });
            }
            return list;
        }

        private static bool TryParseCell(DataGridViewRow row, int col, out double val)
            => double.TryParse(row.Cells[col].Value?.ToString(), out val) && val > 0;

        private void ShowRowError(int rowIdx, string msg)
        {
            dgvLenses.ClearSelection();
            dgvLenses.Rows[rowIdx].Selected = true;
            MessageBox.Show($"행 {rowIdx + 1}: {msg}", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ─────────────────────────────────────────────
        //  Theme
        // ─────────────────────────────────────────────
        private void ApplyTheme()
        {
            Color dark     = Color.FromArgb(12, 14, 18);
            Color border   = Color.FromArgb(40, 55, 80);
            Color fg       = Color.FromArgb(200, 210, 220);
            Color accent   = Color.FromArgb(0, 220, 180);
            Color btnBg    = Color.FromArgb(25, 42, 75);
            Color inputBg  = Color.FromArgb(18, 22, 32);
            Color headerBg = Color.FromArgb(30, 40, 60);

            BackColor = dark;
            pnlButtons.BackColor = Color.FromArgb(18, 24, 36);

            dgvLenses.BackgroundColor = inputBg;
            dgvLenses.GridColor = border;
            dgvLenses.BorderStyle = BorderStyle.None;
            dgvLenses.EnableHeadersVisualStyles = false;
            dgvLenses.DefaultCellStyle.BackColor = inputBg;
            dgvLenses.DefaultCellStyle.ForeColor = fg;
            dgvLenses.DefaultCellStyle.SelectionBackColor = Color.FromArgb(35, 55, 90);
            dgvLenses.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvLenses.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
            dgvLenses.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvLenses.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvLenses.ColumnHeadersDefaultCellStyle.BackColor = headerBg;
            dgvLenses.ColumnHeadersDefaultCellStyle.ForeColor = accent;
            dgvLenses.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvLenses.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvLenses.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(20, 26, 36);

            foreach (Button btn in new[] { btnAdd, btnDelete, btnSave, btnCancel })
            {
                btn.BackColor = btnBg; btn.ForeColor = accent;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = border;
                btn.Font = new Font("Segoe UI", 9f);
            }
            btnSave.BackColor   = Color.FromArgb(20, 60, 40);
            btnSave.ForeColor   = Color.FromArgb(80, 230, 140);
            btnDelete.ForeColor = Color.FromArgb(220, 100, 80);
        }
    }
}
