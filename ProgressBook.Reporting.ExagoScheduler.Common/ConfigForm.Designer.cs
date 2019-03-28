namespace ProgressBook.Reporting.ExagoScheduler.Common
{
    using ProgressBook.Reporting.ExagoScheduler.Common.Controls;

    partial class ConfigForm
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
            this.tabs = new System.Windows.Forms.TabControl();
            this.emailTab = new System.Windows.Forms.TabPage();
            this.emailSettings = new ProgressBook.Reporting.ExagoScheduler.Common.Controls.EmailSettings();
            this.databaseTab = new System.Windows.Forms.TabPage();
            this.databaseSettings = new ProgressBook.Reporting.ExagoScheduler.Common.Controls.DatabaseSettings();
            this.serviceTab = new System.Windows.Forms.TabPage();
            this.serviceAccountSettings = new ProgressBook.Reporting.ExagoScheduler.Common.Controls.ServiceAccountSettings();
            this.advancedTab = new System.Windows.Forms.TabPage();
            this.advancedSettings = new ProgressBook.Reporting.ExagoScheduler.Common.Controls.AdvancedSettings();
            this.ftpLoggingTab = new System.Windows.Forms.TabPage();
            this.ftpLoggingSettings = new ProgressBook.Reporting.ExagoScheduler.Common.Controls.FtpLoggingSettings();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabs.SuspendLayout();
            this.emailTab.SuspendLayout();
            this.databaseTab.SuspendLayout();
            this.serviceTab.SuspendLayout();
            this.advancedTab.SuspendLayout();
            this.ftpLoggingTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.emailTab);
            this.tabs.Controls.Add(this.databaseTab);
            this.tabs.Controls.Add(this.serviceTab);
            this.tabs.Controls.Add(this.advancedTab);
            this.tabs.Controls.Add(this.ftpLoggingTab);
            this.tabs.Location = new System.Drawing.Point(14, 14);
            this.tabs.Name = "tabs";
            this.tabs.Padding = new System.Drawing.Point(10, 5);
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(466, 354);
            this.tabs.TabIndex = 5;
            // 
            // emailTab
            // 
            this.emailTab.Controls.Add(this.emailSettings);
            this.emailTab.Location = new System.Drawing.Point(4, 28);
            this.emailTab.Name = "emailTab";
            this.emailTab.Padding = new System.Windows.Forms.Padding(3);
            this.emailTab.Size = new System.Drawing.Size(458, 322);
            this.emailTab.TabIndex = 1;
            this.emailTab.Text = "Email";
            this.emailTab.UseVisualStyleBackColor = true;
            // 
            // emailSettings
            // 
            this.emailSettings.EmailAddendum = "";
            this.emailSettings.EnableSsl = false;
            this.emailSettings.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emailSettings.FromEmail = "";
            this.emailSettings.FromName = "ProgressBook Ad Hoc Reports Scheduler";
            this.emailSettings.Location = new System.Drawing.Point(3, 3);
            this.emailSettings.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.emailSettings.Name = "emailSettings";
            this.emailSettings.Password = "";
            this.emailSettings.ServerName = "";
            this.emailSettings.Size = new System.Drawing.Size(532, 339);
            this.emailSettings.TabIndex = 4;
            this.emailSettings.UserName = "";
            // 
            // databaseTab
            // 
            this.databaseTab.Controls.Add(this.databaseSettings);
            this.databaseTab.Location = new System.Drawing.Point(4, 28);
            this.databaseTab.Name = "databaseTab";
            this.databaseTab.Padding = new System.Windows.Forms.Padding(3);
            this.databaseTab.Size = new System.Drawing.Size(458, 322);
            this.databaseTab.TabIndex = 0;
            this.databaseTab.Text = "Database";
            this.databaseTab.UseVisualStyleBackColor = true;
            // 
            // databaseSettings
            // 
            this.databaseSettings.ConnectionString = "Data Source=;Initial Catalog=;User ID=;Password=;Application Name=\"Exago Schedule" +
    "r\"";
            this.databaseSettings.DatabaseName = "";
            this.databaseSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databaseSettings.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.databaseSettings.Location = new System.Drawing.Point(3, 3);
            this.databaseSettings.Name = "databaseSettings";
            this.databaseSettings.Password = "";
            this.databaseSettings.ServerName = "";
            this.databaseSettings.Size = new System.Drawing.Size(452, 316);
            this.databaseSettings.TabIndex = 2;
            this.databaseSettings.UserName = "";
            // 
            // serviceTab
            // 
            this.serviceTab.Controls.Add(this.serviceAccountSettings);
            this.serviceTab.Location = new System.Drawing.Point(4, 28);
            this.serviceTab.Name = "serviceTab";
            this.serviceTab.Padding = new System.Windows.Forms.Padding(3);
            this.serviceTab.Size = new System.Drawing.Size(458, 322);
            this.serviceTab.TabIndex = 3;
            this.serviceTab.Text = "Service";
            this.serviceTab.UseVisualStyleBackColor = true;
            // 
            // serviceAccountSettings
            // 
            this.serviceAccountSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serviceAccountSettings.Location = new System.Drawing.Point(3, 3);
            this.serviceAccountSettings.Name = "serviceAccountSettings";
            this.serviceAccountSettings.ServiceAccount = "";
            this.serviceAccountSettings.ServicePassword = "";
            this.serviceAccountSettings.Size = new System.Drawing.Size(452, 316);
            this.serviceAccountSettings.TabIndex = 0;
            // 
            // advancedTab
            // 
            this.advancedTab.AutoScroll = true;
            this.advancedTab.Controls.Add(this.advancedSettings);
            this.advancedTab.Location = new System.Drawing.Point(4, 28);
            this.advancedTab.Name = "advancedTab";
            this.advancedTab.Padding = new System.Windows.Forms.Padding(3);
            this.advancedTab.Size = new System.Drawing.Size(458, 322);
            this.advancedTab.TabIndex = 2;
            this.advancedTab.Text = "Advanced";
            this.advancedTab.UseVisualStyleBackColor = true;
            // 
            // advancedSettings
            // 
            this.advancedSettings.DefaultJobTimeout = 3600;
            this.advancedSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advancedSettings.EmailRetryTime = 10;
            this.advancedSettings.ErrorEmail = "";
            this.advancedSettings.FlushTime = 0;
            this.advancedSettings.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.advancedSettings.Location = new System.Drawing.Point(3, 3);
            this.advancedSettings.Name = "advancedSettings";
            this.advancedSettings.ReportPath = "";
            this.advancedSettings.SimultaneousJobMax = 1;
            this.advancedSettings.Size = new System.Drawing.Size(452, 316);
            this.advancedSettings.SleepTime = 15;
            this.advancedSettings.StopScheduleOnError = false;
            this.advancedSettings.TabIndex = 5;
            // 
            // ftpLoggingTab
            // 
            this.ftpLoggingTab.AutoScroll = true;
            this.ftpLoggingTab.Controls.Add(this.ftpLoggingSettings);
            this.ftpLoggingTab.Location = new System.Drawing.Point(4, 28);
            this.ftpLoggingTab.Name = "ftpLoggingTab";
            this.ftpLoggingTab.Padding = new System.Windows.Forms.Padding(3);
            this.ftpLoggingTab.Size = new System.Drawing.Size(458, 322);
            this.ftpLoggingTab.TabIndex = 4;
            this.ftpLoggingTab.Text = "FTP Logging";
            this.ftpLoggingTab.UseVisualStyleBackColor = true;
            // 
            // ftpLoggingSettings
            // 
            this.ftpLoggingSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ftpLoggingSettings.EnableFtpSessionLogging = false;
            this.ftpLoggingSettings.FtpSessionLogPath = "c:\\temp\\logs\\";
            this.ftpLoggingSettings.Location = new System.Drawing.Point(3, 3);
            this.ftpLoggingSettings.Name = "ftpLoggingSettings";
            this.ftpLoggingSettings.NumberofLogDaysHistoryToMaintain = 10;
            this.ftpLoggingSettings.Size = new System.Drawing.Size(452, 316);
            this.ftpLoggingSettings.TabIndex = 0;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(492, 380);
            this.Controls.Add(this.tabs);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ConfigForm";
            this.Text = "Exago Scheduler Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigForm_FormClosing);
            this.tabs.ResumeLayout(false);
            this.emailTab.ResumeLayout(false);
            this.databaseTab.ResumeLayout(false);
            this.serviceTab.ResumeLayout(false);
            this.advancedTab.ResumeLayout(false);
            this.ftpLoggingTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage databaseTab;
        private Controls.DatabaseSettings databaseSettings;
        private System.Windows.Forms.TabPage emailTab;
        private Controls.EmailSettings emailSettings;
        private System.Windows.Forms.TabPage advancedTab;
        private Controls.AdvancedSettings advancedSettings;
        private System.Windows.Forms.TabPage serviceTab;
        private Controls.ServiceAccountSettings serviceAccountSettings;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabPage ftpLoggingTab;
        private FtpLoggingSettings ftpLoggingSettings;
    }
}