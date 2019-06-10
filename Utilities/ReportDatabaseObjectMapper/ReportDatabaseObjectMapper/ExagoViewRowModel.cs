using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewsExagoDependencies
{
    class ExagoViewRowModel
    {
        public string databaseObjectName { get; set; }
        public string reportName { get; set; }

        public List<string> ToList()
        {
            return new List<string>() {databaseObjectName, reportName};
        }
    }
}
