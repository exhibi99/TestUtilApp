namespace TestUtilApp.UI
{
    partial class LensEditorForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvLenses       = new System.Windows.Forms.DataGridView();
            this.colLensName      = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colManufacturer  = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFocalLength   = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMagnification = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMinWD         = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colImageCircle   = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlButtons      = new System.Windows.Forms.Panel();
            this.btnAdd          = new System.Windows.Forms.Button();
            this.btnDelete       = new System.Windows.Forms.Button();
            this.btnSave         = new System.Windows.Forms.Button();
            this.btnCancel       = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvLenses)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();

            // ── dgvLenses
            this.dgvLenses.Anchor = System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.dgvLenses.Location = new System.Drawing.Point(0, 0);
            this.dgvLenses.Name = "dgvLenses";
            this.dgvLenses.Size = new System.Drawing.Size(860, 460);
            this.dgvLenses.TabIndex = 0;
            this.dgvLenses.RowHeadersVisible = true;
            this.dgvLenses.RowHeadersWidth = 44;
            this.dgvLenses.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLenses.AllowUserToAddRows = false;
            this.dgvLenses.AllowUserToDeleteRows = false;
            this.dgvLenses.AllowUserToResizeRows = false;
            this.dgvLenses.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[]
                { this.colLensName, this.colManufacturer, this.colFocalLength, this.colMagnification, this.colMinWD, this.colImageCircle });

            this.colLensName.HeaderText      = "렌즈명";           this.colLensName.Name      = "colLensName";      this.colLensName.Width      = 220;
            this.colManufacturer.HeaderText  = "제조사";           this.colManufacturer.Name  = "colManufacturer";  this.colManufacturer.Width  = 100;
            this.colFocalLength.HeaderText   = "초점거리 (mm)";    this.colFocalLength.Name   = "colFocalLength";   this.colFocalLength.Width   = 100;
            this.colMagnification.HeaderText = "배율 (0=미사용)";  this.colMagnification.Name = "colMagnification"; this.colMagnification.Width = 110;
            this.colMinWD.HeaderText         = "최소 WD (mm)";     this.colMinWD.Name         = "colMinWD";         this.colMinWD.Width         = 100;
            this.colImageCircle.HeaderText   = "이미지서클 (mm)";  this.colImageCircle.Name   = "colImageCircle";   this.colImageCircle.Width   = 100;

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
            this.pnlButtons.Size = new System.Drawing.Size(860, 44);
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

            this.btnCancel.Location = new System.Drawing.Point(756, 8);
            this.btnCancel.Name     = "btnCancel";
            this.btnCancel.Size     = new System.Drawing.Size(90, 28);
            this.btnCancel.Text     = "취소";
            this.btnCancel.Click   += new System.EventHandler(this.btnCancel_Click);

            this.btnSave.Location   = new System.Drawing.Point(658, 8);
            this.btnSave.Name       = "btnSave";
            this.btnSave.Size       = new System.Drawing.Size(90, 28);
            this.btnSave.Text       = "저장";
            this.btnSave.Click     += new System.EventHandler(this.btnSave_Click);

            // ── Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 504);
            this.Controls.Add(this.dgvLenses);
            this.Controls.Add(this.pnlButtons);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(640, 400);
            this.Name = "LensEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "렌즈 카탈로그 편집";

            ((System.ComponentModel.ISupportInitialize)(this.dgvLenses)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvLenses;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLensName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colManufacturer;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFocalLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMagnification;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMinWD;
        private System.Windows.Forms.DataGridViewTextBoxColumn colImageCircle;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
