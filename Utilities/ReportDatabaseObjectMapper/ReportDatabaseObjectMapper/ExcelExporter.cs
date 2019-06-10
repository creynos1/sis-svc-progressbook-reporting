using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClosedXML.Excel;

namespace ViewsExagoDependencies
{
    class ExcelExporter
    {
        public static void WriteRows(string path, string title, IEnumerable<List<string>> data, List<string> header = null)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(title);

                int rowStart = 1;
                if(header != null)
                {
                    AddHeader(worksheet, header);
                    rowStart++;
                }

                int rowNumber = rowStart;
                foreach(var row in data)
                {
                    var columnNumber = 1;
                    foreach(var value in row)
                    {
                        worksheet.Cell(rowNumber, columnNumber).Value = value;
                        columnNumber++;
                    }
                    rowNumber++;
                }

                workbook.SaveAs(path);
            }
        }

        private static void AddHeader(IXLWorksheet worksheet, List<string> header)
        {
            var columnNumber = 1;
            foreach (var label in header)
            {
                //width unit is ex
                worksheet.Column(columnNumber).Width = 50;
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Cell(1, columnNumber).Value = label;
                columnNumber++;
            }
        }
    }
}
