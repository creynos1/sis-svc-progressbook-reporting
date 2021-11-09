using System;
using System.IO;
using ExagoReportUtility.DataAccessObjects;

namespace ExagoReportUtility.Objects
{
    class InputHandler
    {
        private  string _sqlPath { get; set; }
        private readonly string _finalScriptPath;

        public InputHandler()
        {
            
            DisplayWelcomeMessage();
            ProcessUserFileInput(GetFilePath());
            _finalScriptPath = ".\\FixedExagoDefaultAndSort.sql";
           
        }



        private void DisplayWelcomeMessage()
        {
            Console.WriteLine("Exago Filter Utility.");
            Console.Write("Enter a path to .sql file that constructs reference table [leave blank for default file]: ");
        }

        private string GetFilePath()
        {
            return Console.ReadLine();
        }

        private void ProcessUserFileInput(string path)
        {
            if (path == string.Empty)
            {
                Console.WriteLine("Using default .sql file");
                path = @".\Table Script\CreateExagoReportFamilarNameTables.sql";
            }

            if (!File.Exists(path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(path + "-- File Not Found.  Exiting program");
                Environment.Exit(1);
            }
            _sqlPath = path;
            Console.WriteLine("Creating Look Up Tables...");
            var sqlCommand = ReadSQLFile(_sqlPath);
            var dataAccess = new DataAccess();
            dataAccess.ExecuteSqlCommand(sqlCommand);
            Console.WriteLine("Look up Tables Created Successfully. Creating a .sql file to rename filters.");
            var renameHelper = new RenameHelper();
            renameHelper.GenerateSqlFile();
            Console.Write("Would you like to run the script? (type 'Y' for yes): ");
            if (Console.ReadKey().Key.Equals('Y'))
            {
                Console.WriteLine("Running 'FixedExagoDefaultAndSort.sql' \n");
                sqlCommand = ReadSQLFile(_finalScriptPath);
                dataAccess.ExecuteSqlCommand(sqlCommand);
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            Environment.Exit(0);
            

        }

        private string ReadSQLFile(string path)
        {
            var sqlCommand = string.Empty;
            using(StreamReader sw = new StreamReader(path))
            {
                sqlCommand = sw.ReadToEnd();
            }
            return sqlCommand;

        }
    }
}
