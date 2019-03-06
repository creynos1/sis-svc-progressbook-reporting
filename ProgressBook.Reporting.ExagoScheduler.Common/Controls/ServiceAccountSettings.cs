namespace ProgressBook.Reporting.ExagoScheduler.Common.Controls
{
    using System.Windows.Forms;

    public partial class ServiceAccountSettings : UserControl
    {
        public ServiceAccountSettings()
        {
            InitializeComponent();
        }

        public string ServiceAccount
        {
            get { return serviceAccountTextBox.Text; }
            set { serviceAccountTextBox.Text = value; }
        }

        public string ServicePassword
        {
            get { return servicePasswordTextBox.Text; }
            set { servicePasswordTextBox.Text = value; }
        }
    }
}