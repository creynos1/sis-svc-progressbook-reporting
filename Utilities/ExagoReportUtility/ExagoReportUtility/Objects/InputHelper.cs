using ExagoReportUtility.Models;
using System;
using System.Text;
using ExagoReportUtility.Enums;
using ExagoReportUtility.DataAccessObjects;
using System.Data.SqlClient;

namespace ExagoReportUtility.Objects
{
    public class InputHelper
    {
        private const string RenameReportFilters = "1). Rename Report Filters";
        private const string RenameReportSorts = "2). Rename Report Sorts";
        private const string RenameReportTitles = "3). Rename Report Titles";
        private const string RenameReportDescriptions = "4). Rename Report Descriptions";
        private readonly string[] Selections = new string[4];
        public User User { get; set; }
        public Excel ExcelHelper { get; set; }
        public ExagoReportHelper ReportHelper { get; set; }
        public SqlQuery SqlQuery { get; set; }
        public InputHelper()
        {
            this.User = new User();
            Selections[0] = RenameReportFilters;
            Selections[1] = RenameReportSorts;
            Selections[2] = RenameReportTitles;
            Selections[3] = RenameReportDescriptions;

        }
        public void DisplayWelomeMessage()
        {
            Console.SetCursorPosition(Console.WindowWidth / 3, Console.CursorTop);
            Console.Write(GetWelcomeMessage());
            Console.WriteLine();
        }


        private string GetWelcomeMessage()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Welcome To Exago Utility");
            return stringBuilder.ToString();
        }

        public void SetConnectionHelper()
        {
            Console.Write("Enter the server name:");
            var dataSource = Console.ReadLine();
            Console.Write("Enter the Database to use:");
            var initalCatalog = Console.ReadLine();
            Console.Write("Enter the username:");
            var userId = Console.ReadLine();
            Console.Write("Enter the password:");
            var password = Console.ReadLine();
            this.User.ConnectionHelper = new ConnectionHelper(dataSource, initalCatalog, userId, password);
        }

        public void DisplayUserSelections()
        {
            Console.Write($"Please Make A Selection\r\n {RenameReportFilters} \r\n {RenameReportSorts} \r\n {RenameReportTitles} \r\n {RenameReportDescriptions}: ");
        }
        public UserSelection GetUserSelection()
        {
            var key = Console.ReadKey();
            int keyParse = 0;
            Console.WriteLine();
            try
            {
                keyParse = int.Parse(key.KeyChar.ToString()) - 1;

            }
            catch (Exception ex)
            {
                string errorMessage = "Incorrect Input";
                Global.DisplayErrorMessage(errorMessage, ex);
            }
            this.User.UserSelection = (UserSelection)keyParse;
            var displayMessage = Selections[keyParse];
            Console.Write($"You would like to {displayMessage.Substring(3)} correct ? (Press Y for yes, N for No): ");
            var confirmation = Console.ReadKey().Key;
            if (confirmation == ConsoleKey.Y)
            {
                Console.WriteLine();
                return this.User.UserSelection;
            }
            else
            {
                Console.WriteLine("Application Ending");
                Environment.Exit(0);
            }
            return 0;
        }

        public void SetExcelPathLocation()
        {
            Console.Write("Please Enter the Path for the Excel File: ");
            this.User.ExcelPath = Console.ReadLine();

        }
        public void LoadExcelFile()
        {
            this.ExcelHelper = new Excel(this.User.ExcelPath, (int)this.User.UserSelection);
            this.ReportHelper = new ExagoReportHelper();
            this.SqlQuery = new SqlQuery(this.ReportHelper);
            var rowCount = ExcelHelper.GetRowCount();
            switch (this.User.UserSelection)
            {
                case (UserSelection.RENAMEFILTERS):
                    {
                        MapReportFilters();
                        break;
                    }
                case (UserSelection.RENAMESORTS):
                    {
                        MapReportSorts();
                        break;
                    }
                case (UserSelection.RENAMETITLES):
                    {
                        MapReportTitles(rowCount);
                        break;
                    }
                case (UserSelection.RENAMEDESCRIPTIONS):
                    {
                        MapReportDescriptions(rowCount);
                        break;
                    }
            }
        }

        private void MapReportDescriptions(int rowCount)
        {
            for (int i = 2; i <= rowCount; i++)
            {
                var reportDescription = new ReportDescription();
                var reportTitle = new ReportTitle(this.ExcelHelper.ReadCell<string>(i, 1));
                reportDescription.SoftwareAnswersDescription = this.ExcelHelper.ReadCell<string>(i, 2);
                reportDescription.ReportTitle = reportTitle;
                this.ReportHelper.ReportDescriptions.Add(reportDescription);
                this.ReportHelper.ReportTitles.Add(reportTitle);
            }
        }

        private void MapReportTitles(int rowCount)
        {
            // Static Columns.  Must be in this format
            // Column index starts at 1
            // Row index starts at 2 (1 is the header)

            for (int i = 2; i <= rowCount; i++)
            {
                var reportTitle = new ReportTitle
                {
                    DatabaseTitle = this.ExcelHelper.ReadCell<string>(i, 1),
                    SoftwareAnswersReportTitle = this.ExcelHelper.ReadCell<string>(i, 2),
                    HasIncorrectTitleOnReport = this.ExcelHelper.ReadCell<bool>(i, 3),
                    ReportContentToUpdate = this.ExcelHelper.ReadCell<string>(i, 4)
                };

                this.ReportHelper.ReportTitles.Add(reportTitle);

            }

        }

        private void MapReportSorts()
        {
            throw new NotImplementedException();
        }

        private void MapReportFilters()
        {
            throw new NotImplementedException();
        }


        private void LoadReports()
        {
            try
            {
                using (var sqlConnection = this.User.ConnectionHelper.GetSqlConnection())
                using (var sqlCommand = new SqlCommand(this.SqlQuery.GetReportSqlQuery(), sqlConnection))
                {
                    sqlConnection.Open();
                    var sqlReader = sqlCommand.ExecuteReader();
                    while (sqlReader.Read())
                    {
                        var reportEntity = new ReportEntity
                        {
                            ReportEntityId = sqlReader[sqlReader.GetOrdinal("reportentityid")].ToString(),
                            Content = sqlReader[sqlReader.GetOrdinal("content")].ToString(),
                            Description = sqlReader[sqlReader.GetOrdinal("description")].ToString(),
                            Name = sqlReader[sqlReader.GetOrdinal("name")].ToString()

                        };
                        this.ReportHelper.ReportEntityList.Add(reportEntity);
                        this.ReportHelper.ReportTitleDictionary.Add(reportEntity.Name, reportEntity);
                    }
                }


            }
            catch (Exception ex)
            {
                this.ExcelHelper.Dispose();
                var errorMessage = "An Error has occured while connecting to the SQL Database";
                Global.DisplayErrorMessage(errorMessage, ex);
            }
        }


        public void RenameTitles(UserSelection userSelection)
        {

            LoadReports();
            var outputHelper = new OutputHelper(this.ReportHelper);
            outputHelper.GenerateSqlFile(userSelection);
            outputHelper.RemoveXmlDeclarations();
            Console.WriteLine("Application will Close on Key Press");
            Console.ReadKey();
            this.ExcelHelper.Dispose();

        }

        public void RenameDescriptions(UserSelection userSelection)
        {
            LoadReports();
            var outputHelper = new OutputHelper(this.ReportHelper);
            outputHelper.GenerateSqlFile(userSelection);
            outputHelper.RemoveXmlDeclarations();
            Console.WriteLine("Application will close on key press");
            Console.ReadKey();
            this.ExcelHelper.Dispose();
        }
    }


}
