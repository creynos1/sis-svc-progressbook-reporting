using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using ExagoReportUtility.Objects;

namespace ExagoReportUtility.DataAccessObjects
{
    /// <summary>
    /// Data access class.
    /// </summary>
    class DataAccess
    {
        private string _connectionString;

        public DataAccess()
        {

            _connectionString = ConfigurationManager.ConnectionStrings["StudentInformation"].ConnectionString;

        }

        public SqlConnection GetSQLConnection()

        {
            return new SqlConnection(_connectionString);
        }


        /// <summary>
        /// Create DataTable from Sql Query
        /// </summary>
        /// <param name="SqlQuery">SQL Statement</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(string sqlQuery)
        {
            var dataTable = new DataTable();
            try
            {
                using (var sqlConnection = GetSQLConnection())
                using (SqlCommand command = sqlConnection.CreateCommand())
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {
                    command.CommandText = sqlQuery;
                    command.CommandType = CommandType.Text;
                    sqlConnection.Open();
                    dataAdapter.Fill(dataTable);
                }
            }
            catch (Exception e)
            {
                Global.log.Info(e);
            }
                return dataTable;
        }

        /// <summary>
        /// Executes a SQL NonQuery
        /// </summary>
        /// <param name="sqlNonQuery"></param>
        public void ExecuteSqlCommand(string sqlNonQuery)

        {
            

            try
            {
            using (var sqlConnection = GetSQLConnection())
            using(SqlCommand command = sqlConnection.CreateCommand())
            {
                command.CommandText = sqlNonQuery;
                command.CommandType = CommandType.Text;
                sqlConnection.Open();
                command.ExecuteNonQuery();
                    
            }

            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error While Executing SQL Command.");
                Console.WriteLine("Error Message - " + e.Message);
                Global.log.Fatal(e);
                Console.WriteLine("Press Any Key to Exit");
                Console.ReadKey();
                Environment.Exit(1);
               
            }
        }
                
    }
}
