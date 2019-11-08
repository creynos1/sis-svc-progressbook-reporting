using System;
using System.Configuration;
using System.Data.SqlClient;
using ExagoReportUtility.Objects;

namespace ExagoReportUtility.DataAccessObjects
{
    public class ConnectionHelper
    {
        public string DataSource { get; set; }
        public string InitialCatalog { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get; set; }

        private readonly ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["StudentInformation"];
        public ConnectionHelper(string server, string databaseName, string userId, string password)
        {
            string connectionString = settings.ConnectionString;
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString)
                {
                    DataSource = server,
                    InitialCatalog = databaseName,
                    UserID = userId,
                    Password = password
                };
                this.ConnectionString = builder.ToString();
            }
            catch(Exception ex)
            {
                string message = "Incorrect input parameters";
                Global.DisplayErrorMessage(message, ex);
            }
        }
        public ConnectionHelper()
        {

        }


        public SqlConnection GetSqlConnection()
        {
            return new SqlConnection(this.ConnectionString);
        }
    }
}