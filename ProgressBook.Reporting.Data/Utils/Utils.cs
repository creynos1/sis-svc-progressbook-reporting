using ProgressBook.Reporting.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressBook.Reporting.Data
{
    public static class Utils
    {
        public static string ExagoDataTypeConvert(string dataObject, string sqlType) {
            switch (sqlType.ToLower()) {
                case "money":
                case "decimal":
                case "numeric":
                    return "decimal";
                case "int":
                case "smallint":
                case "tinyint":
                    return "int";
                case "timestamp":
                case "varbinary":
                case "varchar":
                case "char":
                case "nvarchar":
                case "text":
                case "image":
                    return "string";
                case "datetime":
                case "smalldatetime":
                    return "datetime";
                case "date":
                    return "date";
                case "uniqueidentifier":
                    return "guid";
                case "bit":
                    return "boolean";
                default:
                    return "string";
            }            
        }

        public static void Merge(this List<ReportViewInfo> viewInfo, Dictionary<string, List<KeyValuePair<string, string>>> metaInfo) {
            foreach (var item in viewInfo) {
                if (metaInfo.ContainsKey(item.ViewName.ToLower())) {
                    item.MetaColumns = metaInfo[item.ViewName.ToLower()];
                }
            }
        }     
    }
}
