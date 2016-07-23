using System;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Magisterka.Infrastructure.RaportGenerating.ExcelUtilities
{
    public static class CellReferenceConverter
    {
        public static string ToCellReference(this Row row, int columnCount)
        {
            return $"{GetExcelColumnName(columnCount)}{row.RowIndex}";
        }

        private static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = string.Empty;

            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }
    }
}
