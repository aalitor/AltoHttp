namespace DemoApplication
{
    partial class AltoHttpDemoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnPuaseOrResume = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblTotalBytesReceived = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblIsChunked = new System.Windows.Forms.Label();
            this.lblProgress = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblSize = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblFileName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblResumeability = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnChromeIntegration = new System.Windows.Forms.Button();
            this.btnRemoveIntegration = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(13, 12);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(416, 20);
            this.txtUrl.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(13, 40);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // btnPuaseOrResume
            // 
            this.btnPuaseOrResume.Location = new System.Drawing.Point(94, 40);
            this.btnPuaseOrResume.Name = "btnPuaseOrResume";
            this.btnPuaseOrResume.Size = new System.Drawing.Size(75, 23);
            this.btnPuaseOrResume.TabIndex = 1;
            this.btnPuaseOrResume.Text = "Resume";
            this.btnPuaseOrResume.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar);
            this.groupBox1.Controls.Add(this.lblTotalBytesReceived);
            this.groupBox1.Controls.Add(this.lblSpeed);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblIsChunked);
            this.groupBox1.Controls.Add(this.lblProgress);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lblSize);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblFileName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblResumeability);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 93);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(416, 173);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Download Informations";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(25, 133);
            this.progressBar.Maximum = 10000;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(384, 23);
            this.progressBar.TabIndex = 15;
            // 
            // lblTotalBytesReceived
            // 
            this.lblTotalBytesReceived.AutoSize = true;
            this.lblTotalBytesReceived.Location = new System.Drawing.Point(298, 77);
            this.lblTotalBytesReceived.Name = "lblTotalBytesReceived";
            this.lblTotalBytesReceived.Size = new System.Drawing.Size(53, 13);
            this.lblTotalBytesReceived.TabIndex = 3;
            this.lblTotalBytesReceived.Text = "Unknown";
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(298, 47);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(53, 13);
            this.lblSpeed.TabIndex = 4;
            this.lblSpeed.Text = "Unknown";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(209, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Total Received:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(209, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Speed:";
            // 
            // lblIsChunked
            // 
            this.lblIsChunked.AutoSize = true;
            this.lblIsChunked.Location = new System.Drawing.Point(298, 103);
            this.lblIsChunked.Name = "lblIsChunked";
            this.lblIsChunked.Size = new System.Drawing.Size(53, 13);
            this.lblIsChunked.TabIndex = 7;
            this.lblIsChunked.Text = "Unknown";
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(117, 103);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(53, 13);
            this.lblProgress.TabIndex = 7;
            this.lblProgress.Text = "Unknown";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(209, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Is Chunked:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Progress:";
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(117, 75);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(53, 13);
            this.lblSize.TabIndex = 9;
            this.lblSize.Text = "Unknown";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Content Size:";
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(117, 21);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(53, 13);
            this.lblFileName.TabIndex = 11;
            this.lblFileName.Text = "Unknown";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Server File Name:";
            // 
            // lblResumeability
            // 
            this.lblResumeability.AutoSize = true;
            this.lblResumeability.Location = new System.Drawing.Point(117, 47);
            this.lblResumeability.Name = "lblResumeability";
            this.lblResumeability.Size = new System.Drawing.Size(53, 13);
            this.lblResumeability.TabIndex = 13;
            this.lblResumeability.Text = "Unknown";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Supports Resume:";
            // 
            // btnChromeIntegration
            // 
            this.btnChromeIntegration.Location = new System.Drawing.Point(281, 40);
            this.btnChromeIntegration.Name = "btnChromeIntegration";
            this.btnChromeIntegration.Size = new System.Drawing.Size(148, 23);
            this.btnChromeIntegration.TabIndex = 3;
            this.btnChromeIntegration.Text = "Add Chrome Integration";
            this.btnChromeIntegration.UseVisualStyleBackColor = true;
            this.btnChromeIntegration.Click += new System.EventHandler(this.btnChromeIntegration_Click);
            // 
            // btnRemoveIntegration
            // 
            this.btnRemoveIntegration.Location = new System.Drawing.Point(281, 69);
            this.btnRemoveIntegration.Name = "btnRemoveIntegration";
            this.btnRemoveIntegration.Size = new System.Drawing.Size(148, 23);
            this.btnRemoveIntegration.TabIndex = 3;
            this.btnRemoveIntegration.Text = "Remove Chrome Integration";
            this.btnRemoveIntegration.UseVisualStyleBackColor = true;
            this.btnRemoveIntegration.Click += new System.EventHandler(this.btnRemoveIntegration_Click);
            // 
            // AltoHttpDemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 272);
            this.Controls.Add(this.btnRemoveIntegration);
            this.Controls.Add(this.btnChromeIntegration);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnPuaseOrResume);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtUrl);
            this.Name = "AltoHttpDemoForm";
            this.Text = "AltoHttp Demo";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnPuaseOrResume;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblTotalBytesReceived;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblResumeability;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblIsChunked;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnChromeIntegration;
        private System.Windows.Forms.Button btnRemoveIntegration;
    }
}

