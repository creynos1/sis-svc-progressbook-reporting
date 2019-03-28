namespace ProgressBook.Reporting.ExagoScheduler.Common.Controls
{
    partial class FtpLoggingSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ftpSessionLogPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numberOfDaysHistoryToMaintain = new System.Windows.Forms.TextBox();
            this.enableFtpSessionLogging = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.ftpSessionLogPath);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numberOfDaysHistoryToMaintain);
            this.groupBox1.Controls.Add(this.enableFtpSessionLogging);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(357, 167);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FTP Logging Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "FTP Session Log Path:";
            // 
            // ftpSessionLogPath
            // 
            this.ftpSessionLogPath.Location = new System.Drawing.Point(15, 131);
            this.ftpSessionLogPath.Name = "ftpSessionLogPath";
            this.ftpSessionLogPath.Size = new System.Drawing.Size(327, 20);
            this.ftpSessionLogPath.TabIndex = 3;
            this.ftpSessionLogPath.Text = "c:\\temp\\logs\\";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(188, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Number of Days of History to Maintain:";
            // 
            // numberOfDaysHistoryToMaintain
            // 
            this.numberOfDaysHistoryToMaintain.Location = new System.Drawing.Point(209, 65);
            this.numberOfDaysHistoryToMaintain.Name = "numberOfDaysHistoryToMaintain";
            this.numberOfDaysHistoryToMaintain.Size = new System.Drawing.Size(25, 20);
            this.numberOfDaysHistoryToMaintain.TabIndex = 1;
            this.numberOfDaysHistoryToMaintain.Text = "10";
            // 
            // enableFtpSessionLogging
            // 
            this.enableFtpSessionLogging.AutoSize = true;
            this.enableFtpSessionLogging.Checked = true;
            this.enableFtpSessionLogging.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableFtpSessionLogging.Location = new System.Drawing.Point(15, 29);
            this.enableFtpSessionLogging.Name = "enableFtpSessionLogging";
            this.enableFtpSessionLogging.Size = new System.Drawing.Size(163, 17);
            this.enableFtpSessionLogging.TabIndex = 0;
            this.enableFtpSessionLogging.Text = "Enable FTP Session Logging";
            this.enableFtpSessionLogging.UseVisualStyleBackColor = true;
            this.enableFtpSessionLogging.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // FtpLoggingSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "FtpLoggingSettings";
            this.Size = new System.Drawing.Size(357, 167);
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox numberOfDaysHistoryToMaintain;
        private System.Windows.Forms.CheckBox enableFtpSessionLogging;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ftpSessionLogPath;
    }
}
