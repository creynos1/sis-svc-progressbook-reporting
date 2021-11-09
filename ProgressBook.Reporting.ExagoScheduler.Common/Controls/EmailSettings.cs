namespace ProgressBook.Reporting.ExagoScheduler.Common.Controls
{
    public partial class EmailSettings : BaseControl
    {
        public EmailSettings()
        {
            InitializeComponent();

            fromEmailToolTip.SetToolTip(fromEmailTextBox, "Ex: reports@your-domain.com");

            serverNameTextBox.Validating += RequiredFieldHandler;
            fromEmailTextBox.Validating += RequiredEmailFieldHandler;
        }
        
        public string ServerName
        {
            get { return serverNameTextBox.Text; }
            set { serverNameTextBox.Text = value; }
        }

        public string UserName
        {
            get { return userNameTextBox.Text; }
            set { userNameTextBox.Text = value; }
        }

        public string Password
        {
            get { return passwordTextBox.Text; }
            set { passwordTextBox.Text = value; }
        }

        public bool EnableSsl
        {
            get { return enableSslCheckBox.Checked; }
            set { enableSslCheckBox.Checked = value; }
        }
        
        public string FromEmail
        {
            get { return fromEmailTextBox.Text; }
            set { fromEmailTextBox.Text = value; }
        } 

        public string FromName
        {
            get { return fromNameTextBox.Text; }
            set { fromNameTextBox.Text = value; }
        }

        public string EmailAddendum
        {
            get { return emailAddendumTextBox.Text.Replace("\r\n", "\\n"); }
            set { emailAddendumTextBox.Text = value.Replace("\\n", "\r\n"); }
        }
    }
}