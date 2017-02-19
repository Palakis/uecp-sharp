namespace UecpTestClient
{
    partial class MainWindow
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnConnect = new System.Windows.Forms.Button();
            this.tbIPAddr = new System.Windows.Forms.TextBox();
            this.numUdpPort = new System.Windows.Forms.NumericUpDown();
            this.gbControls = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblPI = new System.Windows.Forms.Label();
            this.lblPS = new System.Windows.Forms.Label();
            this.lblRT = new System.Windows.Forms.Label();
            this.tbPI = new System.Windows.Forms.TextBox();
            this.tbPS = new System.Windows.Forms.TextBox();
            this.tbRT = new System.Windows.Forms.TextBox();
            this.btnSetPI = new System.Windows.Forms.Button();
            this.btnSetPS = new System.Windows.Forms.Button();
            this.btnSetRT = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numUdpPort)).BeginInit();
            this.gbControls.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(304, 10);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // tbIPAddr
            // 
            this.tbIPAddr.Location = new System.Drawing.Point(112, 12);
            this.tbIPAddr.Name = "tbIPAddr";
            this.tbIPAddr.Size = new System.Drawing.Size(104, 20);
            this.tbIPAddr.TabIndex = 1;
            // 
            // numUdpPort
            // 
            this.numUdpPort.Location = new System.Drawing.Point(222, 12);
            this.numUdpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numUdpPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numUdpPort.Name = "numUdpPort";
            this.numUdpPort.Size = new System.Drawing.Size(70, 20);
            this.numUdpPort.TabIndex = 2;
            this.numUdpPort.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // gbControls
            // 
            this.gbControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbControls.Controls.Add(this.tableLayoutPanel1);
            this.gbControls.Enabled = false;
            this.gbControls.Location = new System.Drawing.Point(12, 39);
            this.gbControls.Name = "gbControls";
            this.gbControls.Size = new System.Drawing.Size(367, 128);
            this.gbControls.TabIndex = 3;
            this.gbControls.TabStop = false;
            this.gbControls.Text = "RDS Control";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 71F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.Controls.Add(this.lblPI, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblPS, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblRT, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tbPI, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbPS, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbRT, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnSetPI, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSetPS, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnSetRT, 2, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(355, 103);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblPI
            // 
            this.lblPI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPI.AutoSize = true;
            this.lblPI.Location = new System.Drawing.Point(3, 10);
            this.lblPI.Name = "lblPI";
            this.lblPI.Size = new System.Drawing.Size(65, 13);
            this.lblPI.TabIndex = 0;
            this.lblPI.Text = "PI";
            this.lblPI.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPS
            // 
            this.lblPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPS.AutoSize = true;
            this.lblPS.Location = new System.Drawing.Point(3, 43);
            this.lblPS.Name = "lblPS";
            this.lblPS.Size = new System.Drawing.Size(65, 13);
            this.lblPS.TabIndex = 1;
            this.lblPS.Text = "PS";
            this.lblPS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblRT
            // 
            this.lblRT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRT.AutoSize = true;
            this.lblRT.Location = new System.Drawing.Point(3, 76);
            this.lblRT.Name = "lblRT";
            this.lblRT.Size = new System.Drawing.Size(65, 13);
            this.lblRT.TabIndex = 2;
            this.lblRT.Text = "RadioText";
            this.lblRT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbPI
            // 
            this.tbPI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPI.Location = new System.Drawing.Point(74, 6);
            this.tbPI.Name = "tbPI";
            this.tbPI.Size = new System.Drawing.Size(213, 20);
            this.tbPI.TabIndex = 3;
            // 
            // tbPS
            // 
            this.tbPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPS.Location = new System.Drawing.Point(74, 39);
            this.tbPS.Name = "tbPS";
            this.tbPS.Size = new System.Drawing.Size(213, 20);
            this.tbPS.TabIndex = 4;
            // 
            // tbRT
            // 
            this.tbRT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRT.Location = new System.Drawing.Point(74, 72);
            this.tbRT.Name = "tbRT";
            this.tbRT.Size = new System.Drawing.Size(213, 20);
            this.tbRT.TabIndex = 5;
            // 
            // btnSetPI
            // 
            this.btnSetPI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetPI.Location = new System.Drawing.Point(293, 5);
            this.btnSetPI.Name = "btnSetPI";
            this.btnSetPI.Size = new System.Drawing.Size(59, 23);
            this.btnSetPI.TabIndex = 6;
            this.btnSetPI.Text = "Set";
            this.btnSetPI.UseVisualStyleBackColor = true;
            this.btnSetPI.Click += new System.EventHandler(this.btnSetPI_Click);
            // 
            // btnSetPS
            // 
            this.btnSetPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetPS.Location = new System.Drawing.Point(293, 38);
            this.btnSetPS.Name = "btnSetPS";
            this.btnSetPS.Size = new System.Drawing.Size(59, 23);
            this.btnSetPS.TabIndex = 7;
            this.btnSetPS.Text = "Set";
            this.btnSetPS.UseVisualStyleBackColor = true;
            this.btnSetPS.Click += new System.EventHandler(this.btnSetPS_Click);
            // 
            // btnSetRT
            // 
            this.btnSetRT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetRT.Location = new System.Drawing.Point(293, 71);
            this.btnSetRT.Name = "btnSetRT";
            this.btnSetRT.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnSetRT.Size = new System.Drawing.Size(59, 23);
            this.btnSetRT.TabIndex = 8;
            this.btnSetRT.Text = "Set";
            this.btnSetRT.UseVisualStyleBackColor = true;
            this.btnSetRT.Click += new System.EventHandler(this.btnSetRT_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Encoder IP + Port:";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 179);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gbControls);
            this.Controls.Add(this.numUdpPort);
            this.Controls.Add(this.tbIPAddr);
            this.Controls.Add(this.btnConnect);
            this.Name = "MainWindow";
            this.Text = "UECP Client";
            ((System.ComponentModel.ISupportInitialize)(this.numUdpPort)).EndInit();
            this.gbControls.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox tbIPAddr;
        private System.Windows.Forms.NumericUpDown numUdpPort;
        private System.Windows.Forms.GroupBox gbControls;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblPI;
        private System.Windows.Forms.Label lblPS;
        private System.Windows.Forms.Label lblRT;
        private System.Windows.Forms.TextBox tbPI;
        private System.Windows.Forms.TextBox tbPS;
        private System.Windows.Forms.TextBox tbRT;
        private System.Windows.Forms.Button btnSetPI;
        private System.Windows.Forms.Button btnSetPS;
        private System.Windows.Forms.Button btnSetRT;
    }
}

