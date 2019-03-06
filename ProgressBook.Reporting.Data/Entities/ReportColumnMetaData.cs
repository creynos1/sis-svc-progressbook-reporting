using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressBook.Reporting.Data.Entities
{
    public class ReportColumnMetaData
    {
        public string ViewName { get; set; }
        public string SchemaName { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
    }
}
