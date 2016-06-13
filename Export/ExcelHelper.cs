// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcelHelper.cs" company="FreeToDev">Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Export
{
    using System;
    using Microsoft.Office.Interop.Excel;

    public class ExcelHelper
    {
        private static Workbook workbook;
        private static Worksheet worksheet;
        private static Application excelApp;

        public ExcelHelper()
        {
            excelApp = new Application { DisplayAlerts = false };
            workbook = excelApp.Workbooks.Add();
            worksheet = workbook.Sheets.Add();
        }

        public void AddWorksheet(string name)
        {
            worksheet = workbook.Sheets.Add();
            worksheet.Name = name;
        }

        public void WriteHeaderRow(string headings)
        {
            int c = 1;
            foreach (string header in headings.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                worksheet.Cells[1, c] = header;
                c++;
            }
        }

        public void SaveWorkBook(string path, bool append)
        {
            if (append)
            {
                // append code
            }
            else
            {
                workbook.SaveAs(path, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
        }

        public void Close()
        {
            workbook.Close(true);
            excelApp.Quit();
            excelApp = null;
        }

        public void Write(int row, int column, dynamic content)
        {
            worksheet.Cells[row, column] = content;
        }

        public int GetLastRow(string file)
        {
            return worksheet.Cells.Find(
                "*",
                worksheet.Cells[1, 1],
                XlFindLookIn.xlFormulas,
                XlLookAt.xlPart,
                XlSearchOrder.xlByRows,
                XlSearchDirection.xlPrevious,
                Type.Missing,
                Type.Missing,
                Type.Missing).Row + 1;
        }
    }
}
