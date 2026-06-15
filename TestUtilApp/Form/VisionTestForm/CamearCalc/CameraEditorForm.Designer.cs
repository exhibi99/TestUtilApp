namespace TestUtilApp.UI
{
    partial class CameraEditorForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvCameras  = new System.Windows.Forms.DataGridView();
            this.colName     = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSensorX  = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSensorY  = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPixelUm  = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPixelW   = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPixelH   = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlButtons  = new System.Windows.Forms.Panel();
            this.btnAdd      = new System.Windows.Forms.Button();
            this.btnDelete   = new System.Windows.Forms.Button();
            this.btnSave     = new System.Windows.Forms.Button();
            this.btnCancel   = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvCameras)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();

            // ── dgvCameras
            this.dgvCameras.Anchor = System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.dgvCameras.Location = new System.Drawing.Point(0, 0);
            this.dgvCameras.Name = "dgvCameras";
            this.dgvCameras.Size = new System.Drawing.Size(740, 460);
            this.dgvCameras.TabIndex = 0;
            this.dgvCameras.RowHeadersVisible = true;
            this.dgvCameras.RowHeadersWidth = 44;
            this.dgvCameras.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCameras.AllowUserToAddRows = false;
            this.dgvCameras.AllowUserToDeleteRows = false;
            this.dgvCameras.AllowUserToResizeRows = false;
            this.dgvCameras.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[]
                { this.colName, this.colSensorX, this.colSensorY, this.colPixelUm, this.colPixelW, this.colPixelH });

            this.colName.HeaderText    = "카메라명";       this.colName.Name    = "colName";    this.colName.Width    = 220;
            this.colSensorX.HeaderText = "Sensor X (mm)"; this.colSensorX.Name = "colSensorX"; this.colSensorX.Width = 100;
            this.colSensorY.HeaderText = "Sensor Y (mm)"; this.colSensorY.Name = "colSensorY"; this.colSensorY.Width = 100;
            this.colPixelUm.HeaderText = "Pixel (μm)";    this.colPixelUm.Name = "colPixelUm"; this.colPixelUm.Width = 85;
            this.colPixelW.HeaderText  = "Pixel W";       this.colPixelW.Name  = "colPixelW";  this.colPixelW.Width  = 80;
            this.colPixelH.HeaderText  = "Pixel H";       this.colPixelH.Name  = "colPixelH";  this.colPixelH.Width  = 80;

            // ── pnlButtons
            this.pnlButtons.Anchor = System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.pnlButtons.Controls.Add(this.btnAdd);
            this.pnlButtons.Controls.Add(this.btnDelete);
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Location = new System.Drawing.Point(0, 460);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(740, 44);
            this.pnlButtons.TabIndex = 1;

            this.btnAdd.Location    = new System.Drawing.Point(8, 8);
            this.btnAdd.Name        = "btnAdd";
            this.btnAdd.Size        = new System.Drawing.Size(90, 28);
            this.btnAdd.Text        = "+ 행 추가";
            this.btnAdd.Click      += new System.EventHandler(this.btnAdd_Click);

            this.btnDelete.Location = new System.Drawing.Point(106, 8);
            this.btnDelete.Name     = "btnDelete";
            this.btnDelete.Size     = new System.Drawing.Size(90, 28);
            this.btnDelete.Text     = "- 행 삭제";
            this.btnDelete.Click   += new System.EventHandler(this.btnDelete_Click);

            this.btnCancel.Location = new System.Drawing.Point(636, 8);
            this.btnCancel.Name     = "btnCancel";
            this.btnCancel.Size     = new System.Drawing.Size(90, 28);
            this.btnCancel.Text     = "취소";
            this.btnCancel.Click   += new System.EventHandler(this.btnCancel_Click);

            this.btnSave.Location   = new System.Drawing.Point(538, 8);
            this.btnSave.Name       = "btnSave";
            this.btnSave.Size       = new System.Drawing.Size(90, 28);
            this.btnSave.Text       = "저장";
            this.btnSave.Click     += new System.EventHandler(this.btnSave_Click);

            // ── Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 504);
            this.Controls.Add(this.dgvCameras);
            this.Controls.Add(this.pnlButtons);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "CameraEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "카메라 카탈로그 편집";

            ((System.ComponentModel.ISupportInitialize)(this.dgvCameras)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvCameras;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSensorX;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSensorY;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPixelUm;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPixelW;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPixelH;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
