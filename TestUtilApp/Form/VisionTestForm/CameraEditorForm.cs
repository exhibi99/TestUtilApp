using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TestUtilApp.UI
{
    public partial class CameraEditorForm : Form
    {
        private static string ConfigPath  => CameraCalcControl.ConfigJsonPath;
        private static string RuntimePath => CameraCalcControl.RuntimeJsonPath;

        public CameraEditorForm(IEnumerable<CameraCalcControl.CameraEntry> cameras)
        {
            InitializeComponent();
            ApplyTheme();
            LoadRows(cameras);
        }

        private void LoadRows(IEnumerable<CameraCalcControl.CameraEntry> cameras)
        {
            dgvCameras.Rows.Clear();
            foreach (var c in cameras)
                dgvCameras.Rows.Add(c.name, c.sensorX, c.sensorY, c.pixelUm, c.pixelW, c.pixelH);
        }

        // ─────────────────────────────────────────────
        //  Button handlers
        // ─────────────────────────────────────────────
        private void btnAdd_Click(object sender, EventArgs e)
        {
            int idx = dgvCameras.Rows.Add("New Camera", 8.45, 7.07, 3.45, 2448, 2048);
            dgvCameras.ClearSelection();
            dgvCameras.Rows[idx].Selected = true;
            dgvCameras.CurrentCell = dgvCameras.Rows[idx].Cells[0];
            dgvCameras.BeginEdit(true);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvCameras.SelectedRows)
                if (!row.IsNewRow) dgvCameras.Rows.Remove(row);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var list = BuildList();
            if (list == null) return;

            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            try
            {
                File.WriteAllText(RuntimePath, json);

                // 프로젝트 소스 Config도 동기화 (경로가 실제로 존재할 때만)
                string srcConfig = CameraCalcControl.SourceConfigJsonPath;
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
        private List<CameraCalcControl.CameraEntry> BuildList()
        {
            var list = new List<CameraCalcControl.CameraEntry>();
            for (int i = 0; i < dgvCameras.Rows.Count; i++)
            {
                var row = dgvCameras.Rows[i];
                if (row.IsNewRow) continue;

                string name = row.Cells[0].Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(name)) { ShowRowError(i, "카메라명을 입력하세요."); return null; }

                if (!TryParseCell(row, 1, out double sx)) { ShowRowError(i, "Sensor X 값이 올바르지 않습니다."); return null; }
                if (!TryParseCell(row, 2, out double sy)) { ShowRowError(i, "Sensor Y 값이 올바르지 않습니다."); return null; }
                if (!TryParseCell(row, 3, out double pu)) { ShowRowError(i, "Pixel μm 값이 올바르지 않습니다."); return null; }
                if (!TryParseIntCell(row, 4, out int pw)) { ShowRowError(i, "Pixel W 값이 올바르지 않습니다."); return null; }
                if (!TryParseIntCell(row, 5, out int ph)) { ShowRowError(i, "Pixel H 값이 올바르지 않습니다."); return null; }

                list.Add(new CameraCalcControl.CameraEntry
                {
                    name = name, sensorX = sx, sensorY = sy, pixelUm = pu, pixelW = pw, pixelH = ph
                });
            }
            return list;
        }

        private static bool TryParseCell(DataGridViewRow row, int col, out double val)
            => double.TryParse(row.Cells[col].Value?.ToString(), out val) && val > 0;

        private static bool TryParseIntCell(DataGridViewRow row, int col, out int val)
            => int.TryParse(row.Cells[col].Value?.ToString(), out val) && val > 0;

        private void ShowRowError(int rowIdx, string msg)
        {
            dgvCameras.ClearSelection();
            dgvCameras.Rows[rowIdx].Selected = true;
            MessageBox.Show($"행 {rowIdx + 1}: {msg}", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ─────────────────────────────────────────────
        //  Theme
        // ─────────────────────────────────────────────
        private void ApplyTheme()
        {
            Color dark     = Color.FromArgb(12, 14, 18);
            Color panel    = Color.FromArgb(22, 28, 38);
            Color border   = Color.FromArgb(40, 55, 80);
            Color fg       = Color.FromArgb(200, 210, 220);
            Color accent   = Color.FromArgb(0, 220, 180);
            Color btnBg    = Color.FromArgb(25, 42, 75);
            Color inputBg  = Color.FromArgb(18, 22, 32);
            Color headerBg = Color.FromArgb(30, 40, 60);

            BackColor = dark;
            pnlButtons.BackColor = Color.FromArgb(18, 24, 36);

            dgvCameras.BackgroundColor = inputBg;
            dgvCameras.GridColor = border;
            dgvCameras.BorderStyle = BorderStyle.None;
            dgvCameras.EnableHeadersVisualStyles = false;
            dgvCameras.DefaultCellStyle.BackColor = inputBg;
            dgvCameras.DefaultCellStyle.ForeColor = fg;
            dgvCameras.DefaultCellStyle.SelectionBackColor = Color.FromArgb(35, 55, 90);
            dgvCameras.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvCameras.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
            dgvCameras.ColumnHeadersDefaultCellStyle.BackColor = headerBg;
            dgvCameras.ColumnHeadersDefaultCellStyle.ForeColor = accent;
            dgvCameras.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvCameras.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvCameras.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(20, 26, 36);

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
