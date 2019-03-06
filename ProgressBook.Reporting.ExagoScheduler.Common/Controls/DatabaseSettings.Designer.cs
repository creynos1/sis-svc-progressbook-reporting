namespace ProgressBook.Reporting.ExagoScheduler.Common.Controls
{
    partial class DatabaseSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseSettings));
            this.label1 = new System.Windows.Forms.Label();
            this.serverNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.databaseNameTextBox = new System.Windows.Forms.TextBox();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.testDbConnectionButton = new System.Windows.Forms.Button();
            this.greenCheckIcon = new System.Windows.Forms.PictureBox();
            this.redXIcon = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.greenCheckIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.redXIcon)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server Name:";
            // 
            // serverNameTextBox
            // 
            this.serverNameTextBox.Location = new System.Drawing.Point(119, 25);
            this.serverNameTextBox.Name = "serverNameTextBox";
            this.serverNameTextBox.Size = new System.Drawing.Size(186, 21);
            this.serverNameTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Database Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "User:";
            // 
            // databaseNameTextBox
            // 
            this.databaseNameTextBox.Location = new System.Drawing.Point(119, 55);
            this.databaseNameTextBox.Name = "databaseNameTextBox";
            this.databaseNameTextBox.Size = new System.Drawing.Size(186, 21);
            this.databaseNameTextBox.TabIndex = 4;
            // 
            // userNameTextBox
            // 
            this.userNameTextBox.Location = new System.Drawing.Point(119, 85);
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Size = new System.Drawing.Size(186, 21);
            this.userNameTextBox.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Password:";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(118, 115);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(186, 21);
            this.passwordTextBox.TabIndex = 7;
            this.passwordTextBox.UseSystemPasswordChar = true;
            // 
            // testDbConnectionButton
            // 
            this.testDbConnectionButton.Location = new System.Drawing.Point(118, 147);
            this.testDbConnectionButton.Name = "testDbConnectionButton";
            this.testDbConnectionButton.Size = new System.Drawing.Size(122, 27);
            this.testDbConnectionButton.TabIndex = 8;
            this.testDbConnectionButton.Text = "Test Connection";
            this.testDbConnectionButton.UseVisualStyleBackColor = true;
            this.testDbConnectionButton.Click += new System.EventHandler(this.testDbConnectionButton_Click);
            // 
            // greenCheckIcon
            // 
            this.greenCheckIcon.Image = ((System.Drawing.Image)(resources.GetObject("greenCheckIcon.Image")));
            this.greenCheckIcon.InitialImage = null;
            this.greenCheckIcon.Location = new System.Drawing.Point(246, 149);
            this.greenCheckIcon.Name = "greenCheckIcon";
            this.greenCheckIcon.Size = new System.Drawing.Size(24, 24);
            this.greenCheckIcon.TabIndex = 9;
            this.greenCheckIcon.TabStop = false;
            this.greenCheckIcon.Visible = false;
            // 
            // redXIcon
            // 
            this.redXIcon.Image = ((System.Drawing.Image)(resources.GetObject("redXIcon.Image")));
            this.redXIcon.InitialImage = null;
            this.redXIcon.Location = new System.Drawing.Point(246, 149);
            this.redXIcon.Name = "redXIcon";
            this.redXIcon.Size = new System.Drawing.Size(24, 24);
            this.redXIcon.TabIndex = 10;
            this.redXIcon.TabStop = false;
            this.redXIcon.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.redXIcon);
            this.groupBox1.Controls.Add(this.serverNameTextBox);
            this.groupBox1.Controls.Add(this.greenCheckIcon);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.testDbConnectionButton);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.passwordTextBox);
            this.groupBox1.Controls.Add(this.databaseNameTextBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.userNameTextBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(321, 190);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Database";
            // 
            // DatabaseSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DatabaseSettings";
            this.Size = new System.Drawing.Size(321, 190);
            ((System.ComponentModel.ISupportInitialize)(this.greenCheckIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.redXIcon)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox serverNameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox databaseNameTextBox;
        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Button testDbConnectionButton;
        private System.Windows.Forms.PictureBox greenCheckIcon;
        private System.Windows.Forms.PictureBox redXIcon;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
