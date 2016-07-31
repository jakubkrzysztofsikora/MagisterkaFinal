using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Magisterka.Infrastructure.RaportGenerating.ExcelUtilities
{
    public class ExcelWorksheetBuilder
    {
        private static Worksheet _currentlyBuildedWorksheet;
        private static SheetData _currentlyBuildedSheetData;

        public static ExcelWorksheetBuilder StartCreation()
        {
            _currentlyBuildedWorksheet = new Worksheet();
            _currentlyBuildedSheetData = new SheetData();
            return new ExcelWorksheetBuilder();
        }

        public ExcelWorksheetBuilder WithRow(params object[] cellValues)
        {
            ExcelRowBuilder rowBuilder = ExcelRowBuilder.StartCreation();

            rowBuilder = cellValues.Aggregate(rowBuilder, (current, cellValue) => current.WithCell(cellValue.ToString()));
            Row row = rowBuilder.Build();
            _currentlyBuildedSheetData.Append(row);

            return this;
        }

        public ExcelWorksheetBuilder WithSpacingRow()
        {
            Row row = ExcelRowBuilder.StartCreation()
                .WithCell(string.Empty)
                .Build();
            _currentlyBuildedSheetData.Append(row);
            return this;
        }

        public Worksheet Build()
        {
            _currentlyBuildedWorksheet.Append(_currentlyBuildedSheetData);
            return _currentlyBuildedWorksheet;
        }
    }
}
