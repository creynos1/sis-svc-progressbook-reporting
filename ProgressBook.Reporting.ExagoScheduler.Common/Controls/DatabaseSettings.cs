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

            pbMasterServerNameTextBox.Validating += RequiredFieldHandler;
            pbMasterDatabaseNameTextBox.Validating += RequiredFieldHandler;
            pbMasterUserNameTextBox.Validating += RequiredFieldHandler;
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
            get { return BuildConnectionString(true); }
            set { ParseConnectionString(value, true); }
        }

        public string PbMasterServerName
        {
            get { return pbMasterServerNameTextBox.Text; }
            set { pbMasterServerNameTextBox.Text = value; }
        }

        public string PbMasterDatabaseName
        {
            get { return pbMasterDatabaseNameTextBox.Text; }
            set { pbMasterDatabaseNameTextBox.Text = value; }
        }

        public string PbMasterUserName
        {
            get { return pbMasterUserNameTextBox.Text; }
            set { pbMasterUserNameTextBox.Text = value; }
        }

        public string PbMasterPassword
        {
            get { return pbMasterPasswordTextBox.Text; }
            set { pbMasterPasswordTextBox.Text = value; }
        }

        public string PbMasterConnectionString
        {
            get { return BuildConnectionString(false);
            }
            set { ParseConnectionString(value, false);
            }
        }

        private void ParseConnectionString(string connectionString, bool isStudentInformation)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            if (isStudentInformation)
            {
                ServerName = builder.DataSource;
                DatabaseName = builder.InitialCatalog;
                UserName = builder.UserID;
                Password = builder.Password;
            }
            else
            {
                PbMasterServerName = builder.DataSource;
                PbMasterDatabaseName = builder.InitialCatalog;
                PbMasterUserName = builder.UserID;
                PbMasterPassword = builder.Password;
            }
        }

        private string BuildConnectionString(bool isStudentInformation)
        {
            if (isStudentInformation)
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
            else
            {
                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = PbMasterServerName,
                    InitialCatalog = PbMasterDatabaseName,
                    UserID = PbMasterUserName,
                    Password = PbMasterPassword,
                    ApplicationName = "Exago Scheduler"
                };
                return builder.ToString();
            }
        }

        private bool CheckConnection(bool isStudentInformation)
        {
            var connectionString = isStudentInformation ? ConnectionString : PbMasterConnectionString;

            using (var conn = new SqlConnection(connectionString))
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

        private void ToggleConnectionStatusIcons(bool result, bool isStudentInformation)
        {
            if (isStudentInformation)
            {
                redXIcon.Visible = false;
                greenCheckIcon.Visible = false;

                greenCheckIcon.Visible = result;
                redXIcon.Visible = !result;
            }
            else
            {
                redXIcon2.Visible = false;
                greenCheckIcon2.Visible = false;

                greenCheckIcon2.Visible = result;
                redXIcon2.Visible = !result;
            }
        }

        private void testDbConnectionButton_Click(object sender, EventArgs e)
        {
            var result = CheckConnection(true);
            ToggleConnectionStatusIcons(result, true);
        }

        private void testPbMasterConnectionButton_Click(object sender, EventArgs e)
        {
            var result = CheckConnection(false);
            ToggleConnectionStatusIcons(result, false);
        }
    }
}