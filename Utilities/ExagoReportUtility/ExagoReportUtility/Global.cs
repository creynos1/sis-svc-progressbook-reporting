using log4net;
using System;

namespace ExagoReportUtility.Objects
{
    public class Global
    {

        public static ILog log = LogManager.GetLogger("Error");
        public static ILog additions = LogManager.GetLogger("Additions");

        public static  void DisplayErrorMessage(string errorMessage, Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage + "\n Application will close on key press" );
            Console.ReadKey();
            log.Error(exception.Message);
            Environment.Exit(1);
        }

    }

}

