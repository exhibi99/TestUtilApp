namespace TestUtilApp.UI
{
    partial class CameraCalcControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.splitMain       = new System.Windows.Forms.SplitContainer();
            this.tabMain         = new System.Windows.Forms.TabControl();
            this.tabReverse      = new System.Windows.Forms.TabPage();
            this.tabForward      = new System.Windows.Forms.TabPage();

            // ── Tab1 reverse controls
            this.pnlCatalogBar    = new System.Windows.Forms.Panel();
            this.lblCatalogInfo   = new System.Windows.Forms.Label();
            this.btnCatalogEdit   = new System.Windows.Forms.Button();
            this.btnCatalogReload = new System.Windows.Forms.Button();
            this.grpRevInput     = new System.Windows.Forms.GroupBox();
            this.lblRevWD        = new System.Windows.Forms.Label();
            this.txtRevWD        = new System.Windows.Forms.TextBox();
            this.lblRevWDUnit    = new System.Windows.Forms.Label();
            this.lblRevFovX      = new System.Windows.Forms.Label();
            this.txtRevFovX      = new System.Windows.Forms.TextBox();
            this.lblRevFovXUnit  = new System.Windows.Forms.Label();
            this.lblRevFovY      = new System.Windows.Forms.Label();
            this.txtRevFovY      = new System.Windows.Forms.TextBox();
            this.lblRevFovYUnit  = new System.Windows.Forms.Label();
            this.lblRevMinFeat   = new System.Windows.Forms.Label();
            this.txtRevMinFeat   = new System.Windows.Forms.TextBox();
            this.lblRevMinFeatUnit = new System.Windows.Forms.Label();
            this.btnRevCalc      = new System.Windows.Forms.Button();
            this.dgvResults      = new System.Windows.Forms.DataGridView();
            this.colCamera       = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFL           = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFovX         = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFovY         = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRes          = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFit          = new System.Windows.Forms.DataGridViewTextBoxColumn();

            // ── Tab2 forward controls (existing)
            this.grpFov          = new System.Windows.Forms.GroupBox();
            this.lblFovWD        = new System.Windows.Forms.Label();
            this.txtFovWD        = new System.Windows.Forms.TextBox();
            this.lblFovWDUnit    = new System.Windows.Forms.Label();
            this.lblFovFL        = new System.Windows.Forms.Label();
            this.txtFovFL        = new System.Windows.Forms.TextBox();
            this.lblFovFLUnit    = new System.Windows.Forms.Label();
            this.lblFovSensorX   = new System.Windows.Forms.Label();
            this.txtFovSensorX   = new System.Windows.Forms.TextBox();
            this.lblFovSensorXUnit = new System.Windows.Forms.Label();
            this.lblFovSensorY   = new System.Windows.Forms.Label();
            this.txtFovSensorY   = new System.Windows.Forms.TextBox();
            this.lblFovSensorYUnit = new System.Windows.Forms.Label();
            this.btnCalcFov      = new System.Windows.Forms.Button();
            this.lblFovXTitle    = new System.Windows.Forms.Label();
            this.lblFovXResult   = new System.Windows.Forms.Label();
            this.lblFovYTitle    = new System.Windows.Forms.Label();
            this.lblFovYResult   = new System.Windows.Forms.Label();

            // ── Diagram panel (right side)
            this.pnlDiagrams     = new System.Windows.Forms.Panel();
            this.lblSideTitle    = new System.Windows.Forms.Label();
            this.pnlSideView     = new System.Windows.Forms.Panel();
            this.lblFrontTitle   = new System.Windows.Forms.Label();
            this.pnlFrontView    = new System.Windows.Forms.Panel();

            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabReverse.SuspendLayout();
            this.tabForward.SuspendLayout();
            this.pnlCatalogBar.SuspendLayout();
            this.grpRevInput.SuspendLayout();
            this.grpFov.SuspendLayout();
            this.pnlDiagrams.SuspendLayout();
            this.SuspendLayout();

            // ── splitMain
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            this.splitMain.Panel1.Controls.Add(this.tabMain);
            this.splitMain.Panel2.Controls.Add(this.pnlDiagrams);
            this.splitMain.Size = new System.Drawing.Size(1180, 700);
            this.splitMain.SplitterDistance = 430;
            this.splitMain.TabIndex = 0;

            // ── tabMain
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Controls.Add(this.tabReverse);
            this.tabMain.Controls.Add(this.tabForward);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabMain.ItemSize = new System.Drawing.Size(140, 26);
            this.tabMain.SelectedIndexChanged += new System.EventHandler(this.tabMain_SelectedIndexChanged);

            // ── tabReverse
            this.tabReverse.Text = "과제 → 장비";
            this.tabReverse.Name = "tabReverse";
            this.tabReverse.UseVisualStyleBackColor = false;
            this.tabReverse.Controls.Add(this.grpRevInput);
            this.tabReverse.Controls.Add(this.pnlCatalogBar);
            this.tabReverse.Controls.Add(this.dgvResults);

            // ── tabForward
            this.tabForward.Text = "장비 → FOV";
            this.tabForward.Name = "tabForward";
            this.tabForward.UseVisualStyleBackColor = false;
            this.tabForward.Controls.Add(this.grpFov);

            // ────────────────────────────────────────────
            //  Tab 1 — reverse input group
            // ────────────────────────────────────────────
            this.grpRevInput.Anchor = ((System.Windows.Forms.AnchorStyles)(
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right));
            this.grpRevInput.Location = new System.Drawing.Point(4, 4);
            this.grpRevInput.Name = "grpRevInput";
            this.grpRevInput.Size = new System.Drawing.Size(400, 220);
            this.grpRevInput.TabStop = false;
            this.grpRevInput.Text = "과제 조건 입력";
            this.grpRevInput.Controls.Add(this.lblRevWD);
            this.grpRevInput.Controls.Add(this.txtRevWD);
            this.grpRevInput.Controls.Add(this.lblRevWDUnit);
            this.grpRevInput.Controls.Add(this.lblRevFovX);
            this.grpRevInput.Controls.Add(this.txtRevFovX);
            this.grpRevInput.Controls.Add(this.lblRevFovXUnit);
            this.grpRevInput.Controls.Add(this.lblRevFovY);
            this.grpRevInput.Controls.Add(this.txtRevFovY);
            this.grpRevInput.Controls.Add(this.lblRevFovYUnit);
            this.grpRevInput.Controls.Add(this.lblRevMinFeat);
            this.grpRevInput.Controls.Add(this.txtRevMinFeat);
            this.grpRevInput.Controls.Add(this.lblRevMinFeatUnit);
            this.grpRevInput.Controls.Add(this.btnRevCalc);

            // WD
            this.lblRevWD.AutoSize = true;
            this.lblRevWD.Location = new System.Drawing.Point(12, 36);
            this.lblRevWD.Text = "작업 거리 (WD) :";

            this.txtRevWD.Location = new System.Drawing.Point(152, 32);
            this.txtRevWD.Name = "txtRevWD";
            this.txtRevWD.Size = new System.Drawing.Size(100, 27);
            this.txtRevWD.Text = "700";

            this.lblRevWDUnit.AutoSize = true;
            this.lblRevWDUnit.Location = new System.Drawing.Point(258, 36);
            this.lblRevWDUnit.Text = "mm";

            // FOV X
            this.lblRevFovX.AutoSize = true;
            this.lblRevFovX.Location = new System.Drawing.Point(12, 76);
            this.lblRevFovX.Text = "검사 영역 가로 :";

            this.txtRevFovX.Location = new System.Drawing.Point(152, 72);
            this.txtRevFovX.Name = "txtRevFovX";
            this.txtRevFovX.Size = new System.Drawing.Size(100, 27);
            this.txtRevFovX.Text = "200";

            this.lblRevFovXUnit.AutoSize = true;
            this.lblRevFovXUnit.Location = new System.Drawing.Point(258, 76);
            this.lblRevFovXUnit.Text = "mm";

            // FOV Y
            this.lblRevFovY.AutoSize = true;
            this.lblRevFovY.Location = new System.Drawing.Point(12, 116);
            this.lblRevFovY.Text = "검사 영역 세로 :";

            this.txtRevFovY.Location = new System.Drawing.Point(152, 112);
            this.txtRevFovY.Name = "txtRevFovY";
            this.txtRevFovY.Size = new System.Drawing.Size(100, 27);
            this.txtRevFovY.Text = "150";

            this.lblRevFovYUnit.AutoSize = true;
            this.lblRevFovYUnit.Location = new System.Drawing.Point(258, 116);
            this.lblRevFovYUnit.Text = "mm";

            // Min feature
            this.lblRevMinFeat.AutoSize = true;
            this.lblRevMinFeat.Location = new System.Drawing.Point(12, 156);
            this.lblRevMinFeat.Text = "최소 검출 크기 :";

            this.txtRevMinFeat.Location = new System.Drawing.Point(152, 152);
            this.txtRevMinFeat.Name = "txtRevMinFeat";
            this.txtRevMinFeat.Size = new System.Drawing.Size(100, 27);
            this.txtRevMinFeat.Text = "0.5";

            this.lblRevMinFeatUnit.AutoSize = true;
            this.lblRevMinFeatUnit.Location = new System.Drawing.Point(258, 156);
            this.lblRevMinFeatUnit.Text = "mm";

            // Button
            this.btnRevCalc.Location = new System.Drawing.Point(152, 186);
            this.btnRevCalc.Name = "btnRevCalc";
            this.btnRevCalc.Size = new System.Drawing.Size(120, 28);
            this.btnRevCalc.TabIndex = 10;
            this.btnRevCalc.Text = "조합 찾기";
            this.btnRevCalc.UseVisualStyleBackColor = false;
            this.btnRevCalc.Click += new System.EventHandler(this.btnRevCalc_Click);

            // ── pnlCatalogBar
            this.pnlCatalogBar.Anchor = ((System.Windows.Forms.AnchorStyles)(
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right));
            this.pnlCatalogBar.Location = new System.Drawing.Point(4, 228);
            this.pnlCatalogBar.Name = "pnlCatalogBar";
            this.pnlCatalogBar.Size = new System.Drawing.Size(400, 30);
            this.pnlCatalogBar.Controls.Add(this.lblCatalogInfo);
            this.pnlCatalogBar.Controls.Add(this.btnCatalogEdit);
            this.pnlCatalogBar.Controls.Add(this.btnCatalogReload);

            this.lblCatalogInfo.AutoSize = true;
            this.lblCatalogInfo.Location = new System.Drawing.Point(4, 7);
            this.lblCatalogInfo.Name = "lblCatalogInfo";
            this.lblCatalogInfo.Text = "카탈로그: 로드 중...";

            this.btnCatalogEdit.Location = new System.Drawing.Point(240, 3);
            this.btnCatalogEdit.Name = "btnCatalogEdit";
            this.btnCatalogEdit.Size = new System.Drawing.Size(70, 24);
            this.btnCatalogEdit.Text = "편집";
            this.btnCatalogEdit.UseVisualStyleBackColor = false;
            this.btnCatalogEdit.Click += new System.EventHandler(this.btnCatalogEdit_Click);

            this.btnCatalogReload.Location = new System.Drawing.Point(316, 3);
            this.btnCatalogReload.Name = "btnCatalogReload";
            this.btnCatalogReload.Size = new System.Drawing.Size(80, 24);
            this.btnCatalogReload.Text = "다시 로드";
            this.btnCatalogReload.UseVisualStyleBackColor = false;
            this.btnCatalogReload.Click += new System.EventHandler(this.btnCatalogReload_Click);

            // ── dgvResults
            this.dgvResults.Anchor = ((System.Windows.Forms.AnchorStyles)(
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right |
                System.Windows.Forms.AnchorStyles.Bottom));
            this.dgvResults.Location = new System.Drawing.Point(4, 262);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.Size = new System.Drawing.Size(400, 420);
            this.dgvResults.TabIndex = 1;
            this.dgvResults.RowHeadersVisible = false;
            this.dgvResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResults.MultiSelect = false;
            this.dgvResults.ReadOnly = true;
            this.dgvResults.AllowUserToAddRows = false;
            this.dgvResults.AllowUserToDeleteRows = false;
            this.dgvResults.AllowUserToResizeRows = false;
            this.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[]
            {
                this.colCamera, this.colFL, this.colFovX, this.colFovY, this.colRes, this.colFit
            });
            this.dgvResults.SelectionChanged += new System.EventHandler(this.dgvResults_SelectionChanged);

            this.colCamera.HeaderText = "카메라";      this.colCamera.Name = "colCamera"; this.colCamera.ReadOnly = true; this.colCamera.FillWeight = 130;
            this.colFL.HeaderText     = "렌즈 FL";    this.colFL.Name     = "colFL";     this.colFL.ReadOnly     = true; this.colFL.FillWeight     = 58;
            this.colFovX.HeaderText   = "FOV X";      this.colFovX.Name   = "colFovX";   this.colFovX.ReadOnly   = true; this.colFovX.FillWeight   = 68;
            this.colFovY.HeaderText   = "FOV Y";      this.colFovY.Name   = "colFovY";   this.colFovY.ReadOnly   = true; this.colFovY.FillWeight   = 68;
            this.colRes.HeaderText    = "분해능";      this.colRes.Name    = "colRes";    this.colRes.ReadOnly    = true; this.colRes.FillWeight    = 88;
            this.colFit.HeaderText    = "OK";          this.colFit.Name    = "colFit";    this.colFit.ReadOnly    = true; this.colFit.FillWeight    = 38;

            // ────────────────────────────────────────────
            //  Tab 2 — forward (existing grpFov)
            // ────────────────────────────────────────────
            this.grpFov.Anchor = ((System.Windows.Forms.AnchorStyles)(
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right));
            this.grpFov.Controls.Add(this.lblFovWD);
            this.grpFov.Controls.Add(this.txtFovWD);
            this.grpFov.Controls.Add(this.lblFovWDUnit);
            this.grpFov.Controls.Add(this.lblFovFL);
            this.grpFov.Controls.Add(this.txtFovFL);
            this.grpFov.Controls.Add(this.lblFovFLUnit);
            this.grpFov.Controls.Add(this.lblFovSensorX);
            this.grpFov.Controls.Add(this.txtFovSensorX);
            this.grpFov.Controls.Add(this.lblFovSensorXUnit);
            this.grpFov.Controls.Add(this.lblFovSensorY);
            this.grpFov.Controls.Add(this.txtFovSensorY);
            this.grpFov.Controls.Add(this.lblFovSensorYUnit);
            this.grpFov.Controls.Add(this.btnCalcFov);
            this.grpFov.Controls.Add(this.lblFovXTitle);
            this.grpFov.Controls.Add(this.lblFovXResult);
            this.grpFov.Controls.Add(this.lblFovYTitle);
            this.grpFov.Controls.Add(this.lblFovYResult);
            this.grpFov.Location = new System.Drawing.Point(4, 4);
            this.grpFov.Name = "grpFov";
            this.grpFov.Size = new System.Drawing.Size(400, 290);
            this.grpFov.TabIndex = 0;
            this.grpFov.TabStop = false;
            this.grpFov.Text = "FOV 계산  ( WD + 렌즈/센서 → FOV )";

            this.lblFovWD.AutoSize = true;
            this.lblFovWD.Location = new System.Drawing.Point(12, 38);
            this.lblFovWD.Text = "WD :";

            this.txtFovWD.Location = new System.Drawing.Point(152, 34);
            this.txtFovWD.Name = "txtFovWD";
            this.txtFovWD.Size = new System.Drawing.Size(110, 27);
            this.txtFovWD.Text = "700";
            this.txtFovWD.TextChanged += new System.EventHandler(this.InputChanged);

            this.lblFovWDUnit.AutoSize = true;
            this.lblFovWDUnit.Location = new System.Drawing.Point(268, 38);
            this.lblFovWDUnit.Text = "mm";

            this.lblFovFL.AutoSize = true;
            this.lblFovFL.Location = new System.Drawing.Point(12, 78);
            this.lblFovFL.Text = "Focal Length :";

            this.txtFovFL.Location = new System.Drawing.Point(152, 74);
            this.txtFovFL.Name = "txtFovFL";
            this.txtFovFL.Size = new System.Drawing.Size(110, 27);
            this.txtFovFL.Text = "8";
            this.txtFovFL.TextChanged += new System.EventHandler(this.InputChanged);

            this.lblFovFLUnit.AutoSize = true;
            this.lblFovFLUnit.Location = new System.Drawing.Point(268, 78);
            this.lblFovFLUnit.Text = "mm";

            this.lblFovSensorX.AutoSize = true;
            this.lblFovSensorX.Location = new System.Drawing.Point(12, 118);
            this.lblFovSensorX.Text = "Sensor Size X :";

            this.txtFovSensorX.Location = new System.Drawing.Point(152, 114);
            this.txtFovSensorX.Name = "txtFovSensorX";
            this.txtFovSensorX.Size = new System.Drawing.Size(110, 27);
            this.txtFovSensorX.Text = "13.13";
            this.txtFovSensorX.TextChanged += new System.EventHandler(this.InputChanged);

            this.lblFovSensorXUnit.AutoSize = true;
            this.lblFovSensorXUnit.Location = new System.Drawing.Point(268, 118);
            this.lblFovSensorXUnit.Text = "mm";

            this.lblFovSensorY.AutoSize = true;
            this.lblFovSensorY.Location = new System.Drawing.Point(12, 158);
            this.lblFovSensorY.Text = "Sensor Size Y :";

            this.txtFovSensorY.Location = new System.Drawing.Point(152, 154);
            this.txtFovSensorY.Name = "txtFovSensorY";
            this.txtFovSensorY.Size = new System.Drawing.Size(110, 27);
            this.txtFovSensorY.Text = "8.76";
            this.txtFovSensorY.TextChanged += new System.EventHandler(this.InputChanged);

            this.lblFovSensorYUnit.AutoSize = true;
            this.lblFovSensorYUnit.Location = new System.Drawing.Point(268, 158);
            this.lblFovSensorYUnit.Text = "mm";

            this.btnCalcFov.Location = new System.Drawing.Point(152, 194);
            this.btnCalcFov.Name = "btnCalcFov";
            this.btnCalcFov.Size = new System.Drawing.Size(110, 30);
            this.btnCalcFov.TabIndex = 10;
            this.btnCalcFov.Text = "Calculate";
            this.btnCalcFov.UseVisualStyleBackColor = false;
            this.btnCalcFov.Click += new System.EventHandler(this.btnCalcFov_Click);

            this.lblFovXTitle.AutoSize = true;
            this.lblFovXTitle.Location = new System.Drawing.Point(12, 240);
            this.lblFovXTitle.Text = "FOV X :";

            this.lblFovXResult.AutoSize = true;
            this.lblFovXResult.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblFovXResult.Location = new System.Drawing.Point(152, 238);
            this.lblFovXResult.Name = "lblFovXResult";
            this.lblFovXResult.Text = "-";

            this.lblFovYTitle.AutoSize = true;
            this.lblFovYTitle.Location = new System.Drawing.Point(12, 266);
            this.lblFovYTitle.Text = "FOV Y :";

            this.lblFovYResult.AutoSize = true;
            this.lblFovYResult.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblFovYResult.Location = new System.Drawing.Point(152, 264);
            this.lblFovYResult.Name = "lblFovYResult";
            this.lblFovYResult.Text = "-";

            // ────────────────────────────────────────────
            //  Diagram panel (right)
            // ────────────────────────────────────────────
            this.pnlDiagrams.Controls.Add(this.lblSideTitle);
            this.pnlDiagrams.Controls.Add(this.pnlSideView);
            this.pnlDiagrams.Controls.Add(this.lblFrontTitle);
            this.pnlDiagrams.Controls.Add(this.pnlFrontView);
            this.pnlDiagrams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDiagrams.Location = new System.Drawing.Point(0, 0);
            this.pnlDiagrams.Name = "pnlDiagrams";
            this.pnlDiagrams.Size = new System.Drawing.Size(746, 700);
            this.pnlDiagrams.TabIndex = 0;

            this.lblSideTitle.AutoSize = false;
            this.lblSideTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSideTitle.Location = new System.Drawing.Point(0, 0);
            this.lblSideTitle.Name = "lblSideTitle";
            this.lblSideTitle.Size = new System.Drawing.Size(746, 22);
            this.lblSideTitle.Text = "  Side View  ( Camera ← WD → Subject )";
            this.lblSideTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.pnlSideView.Anchor = ((System.Windows.Forms.AnchorStyles)(
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right));
            this.pnlSideView.Location = new System.Drawing.Point(0, 22);
            this.pnlSideView.Name = "pnlSideView";
            this.pnlSideView.Size = new System.Drawing.Size(746, 326);
            this.pnlSideView.TabIndex = 0;
            this.pnlSideView.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlSideView_Paint);
            this.pnlSideView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlSideView_MouseDown);
            this.pnlSideView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlSideView_MouseMove);
            this.pnlSideView.MouseUp   += new System.Windows.Forms.MouseEventHandler(this.pnlSideView_MouseUp);

            this.lblFrontTitle.AutoSize = false;
            this.lblFrontTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblFrontTitle.Location = new System.Drawing.Point(0, 352);
            this.lblFrontTitle.Name = "lblFrontTitle";
            this.lblFrontTitle.Size = new System.Drawing.Size(746, 22);
            this.lblFrontTitle.Text = "  Front View  ( FOV at Subject Plane )";
            this.lblFrontTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.pnlFrontView.Anchor = ((System.Windows.Forms.AnchorStyles)(
                System.Windows.Forms.AnchorStyles.Top |
                System.Windows.Forms.AnchorStyles.Left |
                System.Windows.Forms.AnchorStyles.Right));
            this.pnlFrontView.Location = new System.Drawing.Point(0, 374);
            this.pnlFrontView.Name = "pnlFrontView";
            this.pnlFrontView.Size = new System.Drawing.Size(746, 320);
            this.pnlFrontView.TabIndex = 1;
            this.pnlFrontView.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlFrontView_Paint);

            // ── CameraCalcControl
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(12, 14, 18);
            this.Controls.Add(this.splitMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this.Name = "CameraCalcControl";
            this.Size = new System.Drawing.Size(1180, 700);

            this.pnlCatalogBar.ResumeLayout(false);
            this.pnlCatalogBar.PerformLayout();
            this.grpRevInput.ResumeLayout(false);
            this.grpRevInput.PerformLayout();
            this.grpFov.ResumeLayout(false);
            this.grpFov.PerformLayout();
            this.tabReverse.ResumeLayout(false);
            this.tabForward.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.pnlDiagrams.ResumeLayout(false);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabReverse;
        private System.Windows.Forms.TabPage tabForward;

        // Tab 1 – reverse
        private System.Windows.Forms.Panel pnlCatalogBar;
        private System.Windows.Forms.Label lblCatalogInfo;
        private System.Windows.Forms.Button btnCatalogEdit;
        private System.Windows.Forms.Button btnCatalogReload;
        private System.Windows.Forms.GroupBox grpRevInput;
        private System.Windows.Forms.Label lblRevWD;
        private System.Windows.Forms.TextBox txtRevWD;
        private System.Windows.Forms.Label lblRevWDUnit;
        private System.Windows.Forms.Label lblRevFovX;
        private System.Windows.Forms.TextBox txtRevFovX;
        private System.Windows.Forms.Label lblRevFovXUnit;
        private System.Windows.Forms.Label lblRevFovY;
        private System.Windows.Forms.TextBox txtRevFovY;
        private System.Windows.Forms.Label lblRevFovYUnit;
        private System.Windows.Forms.Label lblRevMinFeat;
        private System.Windows.Forms.TextBox txtRevMinFeat;
        private System.Windows.Forms.Label lblRevMinFeatUnit;
        private System.Windows.Forms.Button btnRevCalc;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCamera;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFL;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFovX;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFovY;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFit;

        // Tab 2 – forward
        private System.Windows.Forms.GroupBox grpFov;
        private System.Windows.Forms.Label lblFovWD;
        private System.Windows.Forms.TextBox txtFovWD;
        private System.Windows.Forms.Label lblFovWDUnit;
        private System.Windows.Forms.Label lblFovFL;
        private System.Windows.Forms.TextBox txtFovFL;
        private System.Windows.Forms.Label lblFovFLUnit;
        private System.Windows.Forms.Label lblFovSensorX;
        private System.Windows.Forms.TextBox txtFovSensorX;
        private System.Windows.Forms.Label lblFovSensorXUnit;
        private System.Windows.Forms.Label lblFovSensorY;
        private System.Windows.Forms.TextBox txtFovSensorY;
        private System.Windows.Forms.Label lblFovSensorYUnit;
        private System.Windows.Forms.Button btnCalcFov;
        private System.Windows.Forms.Label lblFovXTitle;
        private System.Windows.Forms.Label lblFovXResult;
        private System.Windows.Forms.Label lblFovYTitle;
        private System.Windows.Forms.Label lblFovYResult;

        // Diagram
        private System.Windows.Forms.Panel pnlDiagrams;
        private System.Windows.Forms.Label lblSideTitle;
        private System.Windows.Forms.Panel pnlSideView;
        private System.Windows.Forms.Label lblFrontTitle;
        private System.Windows.Forms.Panel pnlFrontView;
    }
}
