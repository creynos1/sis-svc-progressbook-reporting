using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewsExagoDependencies
{
    class ExagoViewRowModel
    {
        public string DatabaseObjectName { get; set; }
        public string ReportName { get; set; }
        public string FolderPath { get; set; }

        public List<string> ToList()
        {
            return new List<string>() {DatabaseObjectName, ReportName, FolderPath};
        }
    }
}
