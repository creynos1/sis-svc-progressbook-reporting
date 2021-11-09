namespace ProgressBook.Reporting.ExagoScheduler.Common.Controls
{
    partial class AdvancedSettings
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.defaultJobTimeoutTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.stopScheduleOnErrorCheckBox = new System.Windows.Forms.CheckBox();
            this.errorEmailTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.reportPathTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.emailRetryTimeTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.flushTimeTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.disableAutoFlushCheckBox = new System.Windows.Forms.CheckBox();
            this.simultaneousJobMaxUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.sleepTimeTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.flushTimeToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.emailRetryTimeToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.simultaneousJobMaxUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 132);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Default Job Timeout:";
            // 
            // defaultJobTimeoutTextBox
            // 
            this.defaultJobTimeoutTextBox.Location = new System.Drawing.Point(149, 129);
            this.defaultJobTimeoutTextBox.Name = "defaultJobTimeoutTextBox";
            this.defaultJobTimeoutTextBox.Size = new System.Drawing.Size(46, 21);
            this.defaultJobTimeoutTextBox.TabIndex = 1;
            this.defaultJobTimeoutTextBox.Text = "3600";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.browseButton);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.stopScheduleOnErrorCheckBox);
            this.groupBox1.Controls.Add(this.errorEmailTextBox);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.reportPathTextBox);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.emailRetryTimeTextBox);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.flushTimeTextBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.disableAutoFlushCheckBox);
            this.groupBox1.Controls.Add(this.simultaneousJobMaxUpDown);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.sleepTimeTextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.defaultJobTimeoutTextBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(356, 317);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Advanced Settings";
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(310, 46);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(24, 23);
            this.browseButton.TabIndex = 31;
            this.browseButton.Text = "...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 282);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(137, 15);
            this.label11.TabIndex = 30;
            this.label11.Text = "Stop Schedule on Error:";
            // 
            // stopScheduleOnErrorCheckBox
            // 
            this.stopScheduleOnErrorCheckBox.AutoSize = true;
            this.stopScheduleOnErrorCheckBox.Location = new System.Drawing.Point(149, 282);
            this.stopScheduleOnErrorCheckBox.Name = "stopScheduleOnErrorCheckBox";
            this.stopScheduleOnErrorCheckBox.Size = new System.Drawing.Size(15, 14);
            this.stopScheduleOnErrorCheckBox.TabIndex = 29;
            this.stopScheduleOnErrorCheckBox.UseVisualStyleBackColor = true;
            // 
            // errorEmailTextBox
            // 
            this.errorEmailTextBox.Location = new System.Drawing.Point(10, 99);
            this.errorEmailTextBox.Name = "errorEmailTextBox";
            this.errorEmailTextBox.Size = new System.Drawing.Size(324, 21);
            this.errorEmailTextBox.TabIndex = 28;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 79);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(112, 15);
            this.label8.TabIndex = 27;
            this.label8.Text = "Error Report Email:";
            // 
            // reportPathTextBox
            // 
            this.reportPathTextBox.Location = new System.Drawing.Point(10, 47);
            this.reportPathTextBox.Name = "reportPathTextBox";
            this.reportPathTextBox.Size = new System.Drawing.Size(294, 21);
            this.reportPathTextBox.TabIndex = 26;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 15);
            this.label7.TabIndex = 25;
            this.label7.Text = "Report Path:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(210, 252);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 15);
            this.label9.TabIndex = 24;
            this.label9.Text = "minutes";
            // 
            // emailRetryTimeTextBox
            // 
            this.emailRetryTimeTextBox.Location = new System.Drawing.Point(149, 249);
            this.emailRetryTimeTextBox.Name = "emailRetryTimeTextBox";
            this.emailRetryTimeTextBox.Size = new System.Drawing.Size(46, 21);
            this.emailRetryTimeTextBox.TabIndex = 23;
            this.emailRetryTimeTextBox.Text = "10";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 252);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(104, 15);
            this.label10.TabIndex = 22;
            this.label10.Text = "Email Retry Time:";
            // 
            // flushTimeTextBox
            // 
            this.flushTimeTextBox.Location = new System.Drawing.Point(149, 219);
            this.flushTimeTextBox.Name = "flushTimeTextBox";
            this.flushTimeTextBox.Size = new System.Drawing.Size(46, 21);
            this.flushTimeTextBox.TabIndex = 16;
            this.flushTimeTextBox.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 222);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 15);
            this.label6.TabIndex = 15;
            this.label6.Text = "Flush Time:";
            // 
            // disableAutoFlushCheckBox
            // 
            this.disableAutoFlushCheckBox.AutoSize = true;
            this.disableAutoFlushCheckBox.Location = new System.Drawing.Point(213, 221);
            this.disableAutoFlushCheckBox.Name = "disableAutoFlushCheckBox";
            this.disableAutoFlushCheckBox.Size = new System.Drawing.Size(126, 19);
            this.disableAutoFlushCheckBox.TabIndex = 14;
            this.disableAutoFlushCheckBox.Text = "Disable auto flush";
            this.disableAutoFlushCheckBox.UseVisualStyleBackColor = true;
            this.disableAutoFlushCheckBox.CheckedChanged += new System.EventHandler(this.disableAutoFlushCheckbox_CheckedChanged);
            // 
            // simultaneousJobMaxUpDown
            // 
            this.simultaneousJobMaxUpDown.BackColor = System.Drawing.SystemColors.Window;
            this.simultaneousJobMaxUpDown.Location = new System.Drawing.Point(149, 189);
            this.simultaneousJobMaxUpDown.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.simultaneousJobMaxUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.simultaneousJobMaxUpDown.Name = "simultaneousJobMaxUpDown";
            this.simultaneousJobMaxUpDown.Size = new System.Drawing.Size(47, 21);
            this.simultaneousJobMaxUpDown.TabIndex = 13;
            this.simultaneousJobMaxUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 192);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(134, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "Simultaneous Job Max:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(210, 162);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "seconds";
            // 
            // sleepTimeTextBox
            // 
            this.sleepTimeTextBox.Location = new System.Drawing.Point(149, 159);
            this.sleepTimeTextBox.Name = "sleepTimeTextBox";
            this.sleepTimeTextBox.Size = new System.Drawing.Size(46, 21);
            this.sleepTimeTextBox.TabIndex = 10;
            this.sleepTimeTextBox.Text = "15";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 162);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "Sleep Time:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(210, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "seconds";
            // 
            // AdvancedSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AdvancedSettings";
            this.Size = new System.Drawing.Size(356, 317);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.simultaneousJobMaxUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox defaultJobTimeoutTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolTip flushTimeToolTip;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown simultaneousJobMaxUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox sleepTimeTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox disableAutoFlushCheckBox;
        private System.Windows.Forms.TextBox flushTimeTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox emailRetryTimeTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox errorEmailTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox reportPathTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolTip emailRetryTimeToolTip;
        private System.Windows.Forms.CheckBox stopScheduleOnErrorCheckBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button browseButton;
    }
}
