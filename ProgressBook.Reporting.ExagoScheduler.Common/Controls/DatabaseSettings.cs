namespace ProgressBook.Reporting.ExagoScheduler.Common.Controls
{
    using System;
    using System.Data.SqlClient;

    public partial class DatabaseSettings : BaseControl
    {
        public DatabaseSettings()
        {
            InitializeComponent();

            serverNameTextBox.Validating += RequiredFieldHandler;
            databaseNameTextBox.Validating += RequiredFieldHandler;
            userNameTextBox.Validating += RequiredFieldHandler;
        }

        public string ServerName
        {
            get { return serverNameTextBox.Text; }
            set { serverNameTextBox.Text = value; }
        }

        public string DatabaseName
        {
            get { return databaseNameTextBox.Text; }
            set { databaseNameTextBox.Text = value; }
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

        public string ConnectionString
        {
            get { return BuildConnectionString(); }
            set { ParseConnectionString(value); }
        }

        private void ParseConnectionString(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            ServerName = builder.DataSource;
            DatabaseName = builder.InitialCatalog;
            UserName = builder.UserID;
            Password = builder.Password;
        }

        private string BuildConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ServerName,
                InitialCatalog = DatabaseName,
                UserID = UserName,
                Password = Password,
                ApplicationName = "Exago Scheduler"
            };

            return builder.ToString();
        }

        private bool CheckConnection()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT 1";
                    conn.Open();
                    return (int) cmd.ExecuteScalar() == 1;
                }
                catch (SqlException ex)
                {
                    return false;
                }
            }
        }

        private void ToggleConnectionStatusIcons(bool result)
        {
            redXIcon.Visible = false;
            greenCheckIcon.Visible = false;

            greenCheckIcon.Visible = result;
            redXIcon.Visible = !result;
        }

        private void testDbConnectionButton_Click(object sender, EventArgs e)
        {
            var result = CheckConnection();
            ToggleConnectionStatusIcons(result);
        }
    }
}