using Microsoft.Office.Interop.Excel;
using System;
using _Excel = Microsoft.Office.Interop.Excel;

namespace ExagoReportUtility.Objects
{
    public class Excel
    {
        public string Path { get; set; }

        private readonly _Application excelApp = new Application();
        private readonly Workbook _workbook;
        private readonly Worksheet _worksheet;
        public Excel(string path, int sheet)
        {
            // Inserting this to ensure I type the right path.  Remove 
            path = "c:\\Repo\\Software Answers Filter and Sort Names.xlsx";
            Path = path;
            try
            {
                _workbook = excelApp.Workbooks.Open(Path);
                // Index starts at 1 for Microsoft Excel
                _worksheet = _workbook.Worksheets[++sheet];
            }
            catch (Exception ex)
            {
                string errorMessage = "An error has occured while loading the excel file";
                Global.DisplayErrorMessage(errorMessage, ex);
            }
        }

        public Excel()
        {

        }


        public int GetRowCount()
        {
            _worksheet.Rows.ClearFormats();
            return _worksheet.UsedRange.Rows.Count;
        }


        public T ReadCell<T>(int row, int column)
        {
            try
            {
                return _worksheet.Cells[row, column].Value2;
            }
            catch(Exception ex)
            {
                Global.DisplayErrorMessage("Value is null, and cannot parse", ex);
                throw;
            }

            
        }

        public void Dispose()
        {
            _workbook.Close(0);
            excelApp.Quit();
        }
    }
}
