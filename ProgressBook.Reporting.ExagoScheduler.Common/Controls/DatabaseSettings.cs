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

            specialServicesServerNameTextBox.Validating += RequiredFieldHandler;
            specialServicesDatabaseNameTextBox.Validating += RequiredFieldHandler;
            specialServicesUserNameTextBox.Validating += RequiredFieldHandler;
        }

        public string ServerName
        {
            get => serverNameTextBox.Text;
            set => serverNameTextBox.Text = value;
        }

        public string DatabaseName
        {
            get => databaseNameTextBox.Text;
            set => databaseNameTextBox.Text = value;
        }

        public string UserName
        {
            get => userNameTextBox.Text;
            set => userNameTextBox.Text = value;
        }

        public string Password
        {
            get => passwordTextBox.Text;
            set => passwordTextBox.Text = value;
        }

        public string ConnectionString
        {
            get => BuildConnectionString(Connections.StudentInformation);
            set => ParseConnectionString(value, Connections.StudentInformation);
        }

        public string PbMasterServerName
        {
            get => pbMasterServerNameTextBox.Text;
            set => pbMasterServerNameTextBox.Text = value;
        }

        public string PbMasterDatabaseName
        {
            get => pbMasterDatabaseNameTextBox.Text;
            set => pbMasterDatabaseNameTextBox.Text = value;
        }

        public string PbMasterUserName
        {
            get => pbMasterUserNameTextBox.Text;
            set => pbMasterUserNameTextBox.Text = value;
        }

        public string PbMasterPassword
        {
            get => pbMasterPasswordTextBox.Text;
            set => pbMasterPasswordTextBox.Text = value;
        }

        public string PbMasterConnectionString
        {
            get => BuildConnectionString(Connections.PbMaster);
            set => ParseConnectionString(value, Connections.PbMaster);
        }

        public string SpecialServicesServerName
        {
            get => pbMasterServerNameTextBox.Text;
            set => pbMasterServerNameTextBox.Text = value;
        }

        public string SpecialServicesDatabaseName
        {
            get => specialServicesDatabaseNameTextBox.Text;
            set => specialServicesDatabaseNameTextBox.Text = value;
        }

        public string SpecialServicesUserName
        {
            get => specialServicesUserNameTextBox.Text;
            set => specialServicesUserNameTextBox.Text = value;
        }

        public string SpecialServicesPassword
        {
            get => specialServicesPasswordTextBox.Text;
            set => specialServicesPasswordTextBox.Text = value;
        }

        public string SpecialServicesConnectionString
        {
            get => BuildConnectionString(Connections.SpecialServices);
            set => ParseConnectionString(value, Connections.SpecialServices);
        }

        private void ParseConnectionString(string connectionString, Connections connection)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            switch (connection)
            {
                case Connections.PbMaster:
                    PbMasterServerName = builder.DataSource;
                    PbMasterDatabaseName = builder.InitialCatalog;
                    PbMasterUserName = builder.UserID;
                    PbMasterPassword = builder.Password;
                    break;
                case Connections.SpecialServices:
                    SpecialServicesServerName = builder.DataSource;
                    SpecialServicesDatabaseName = builder.InitialCatalog;
                    SpecialServicesUserName = builder.UserID;
                    SpecialServicesPassword = builder.Password;
                    break;
                default:
                    ServerName = builder.DataSource;
                    DatabaseName = builder.InitialCatalog;
                    UserName = builder.UserID;
                    Password = builder.Password;
                    break;
            }
        }

        private string BuildConnectionString(Connections connection)
        {
            switch (connection)
            {
                case Connections.PbMaster:
                    return new SqlConnectionStringBuilder
                    {
                        DataSource = PbMasterServerName,
                        InitialCatalog = PbMasterDatabaseName,
                        UserID = PbMasterUserName,
                        Password = PbMasterPassword,
                        ApplicationName = "Exago Scheduler"
                    }.ToString();
                case Connections.SpecialServices:
                    return new SqlConnectionStringBuilder()
                    {
                        DataSource = SpecialServicesServerName,
                        InitialCatalog = SpecialServicesDatabaseName,
                        UserID = SpecialServicesUserName,
                        Password = SpecialServicesPassword,
                        ApplicationName = "Exago Scheduler"
                    }.ToString();
                default:
                    return new SqlConnectionStringBuilder
                    {
                        DataSource = ServerName,
                        InitialCatalog = DatabaseName,
                        UserID = UserName,
                        Password = Password,
                        ApplicationName = "Exago Scheduler"
                    }.ToString();
            }
        }

        private bool CheckConnection(Connections connection)
        {
            string connectionString;
            switch (connection)
            {
                case Connections.PbMaster:
                    connectionString = PbMasterConnectionString;
                    break;
                case Connections.SpecialServices:
                    connectionString = SpecialServicesConnectionString;
                    break;
                default:
                    connectionString = ConnectionString;
                    break;
            }

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

        private void ToggleConnectionStatusIcons(bool result, Connections connection)
        {
            switch (connection)
            {
                case Connections.PbMaster:
                    redXIcon2.Visible = false;
                    greenCheckIcon2.Visible = false;

                    greenCheckIcon2.Visible = result;
                    redXIcon2.Visible = !result;
                    break;
                case Connections.SpecialServices:
                    redXIcon3.Visible = false;
                    greenCheckIcon3.Visible = false;

                    greenCheckIcon3.Visible = result;
                    redXIcon3.Visible = !result;
                    break;
                default:
                    redXIcon.Visible = false;
                    greenCheckIcon.Visible = false;

                    greenCheckIcon.Visible = result;
                    redXIcon.Visible = !result;
                    break;
            }
        }

        private void testDbConnectionButton_Click(object sender, EventArgs e)
        {
            var result = CheckConnection(Connections.StudentInformation);
            ToggleConnectionStatusIcons(result, Connections.StudentInformation);
        }

        private void testPbMasterConnectionButton_Click(object sender, EventArgs e)
        {
            var result = CheckConnection(Connections.PbMaster);
            ToggleConnectionStatusIcons(result, Connections.PbMaster);
        }

        private void testSpecialServicesConnectionButton_Click(object sender, EventArgs e)
        {
            var result = CheckConnection(Connections.SpecialServices);
            ToggleConnectionStatusIcons(result, Connections.SpecialServices);
        }
    }

    public enum Connections
    {
        PbMaster,
        SpecialServices,
        StudentInformation,
    }
}