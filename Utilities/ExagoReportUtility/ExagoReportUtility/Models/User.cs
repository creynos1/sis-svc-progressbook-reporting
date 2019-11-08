using ExagoReportUtility.Enums;
using ExagoReportUtility.DataAccessObjects;

namespace ExagoReportUtility.Models
{
 public class User
    {
        public string ExcelPath { get; set; }
        public UserSelection UserSelection { get; set; }
        public ConnectionHelper ConnectionHelper { get; set; }

    }


  
}
