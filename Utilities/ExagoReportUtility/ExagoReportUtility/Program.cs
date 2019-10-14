using ExagoReportUtility.Objects;
using log4net;
using ExagoReportUtility.Enums;

namespace ExagoReportUtility
{
    /// <summary>
    /// This Utility can generate batch SQL files to update
    /// reports.  Filters/Sorts need to be implemented still
    /// These items have started and are in the items to refactor folder
    /// </summary>
    class Program
    {
        public static ILog log = LogManager.GetLogger("Error");
        public static void Main()
        {
            var inputHelper = new InputHelper();
            inputHelper.DisplayWelomeMessage();
            inputHelper.SetConnectionHelper();
            inputHelper.SetExcelPathLocation();
            inputHelper.DisplayUserSelections();
            UserSelection key = inputHelper.GetUserSelection();
            inputHelper.LoadExcelFile();
            switch ((UserSelection)key)
            {
                case (UserSelection.RENAMETITLES):
                    {
                        inputHelper.RenameTitles(key);
                        break;
                    }
                case (UserSelection.RENAMEDESCRIPTIONS):
                    {
                        inputHelper.RenameDescriptions(key);
                        break;
                    }
            }

        }



    }
}
