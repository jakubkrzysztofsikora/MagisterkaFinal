using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Magisterka.Infrastructure.RaportGenerating.ExcelUtilities
{
    public class ExcelRowBuilder
    {
        private static Row _currentlyBuildingRow;
        private static Dictionary<Row, int> _numberOfCellsInRow;

        public static ExcelRowBuilder StartCreation()
        {
            _currentlyBuildingRow = new Row();
            _numberOfCellsInRow = _numberOfCellsInRow ?? new Dictionary<Row, int>();
           _numberOfCellsInRow.Add(_currentlyBuildingRow, 0);

            return new ExcelRowBuilder();
        }

        public ExcelRowBuilder WithCell(string value)
        {
            IncrementRowCellCount();
            Cell cell = new Cell
            {
                CellReference = _currentlyBuildingRow.ToCellReference(_numberOfCellsInRow[_currentlyBuildingRow]),
                DataType = CellValues.InlineString
            };
            InlineString inlineString = new InlineString();
            Text title = new Text(value);
            ChainSpreadsheetElements(cell, inlineString, title);
            return this;
        }

        public ExcelRowBuilder WithCell(long value)
        {
            return WithCell(value.ToString());
        }

        public Row Build()
        {
            return _currentlyBuildingRow;
        }

        private void IncrementRowCellCount()
        {
            ++_numberOfCellsInRow[_currentlyBuildingRow];
        }

        private void ChainSpreadsheetElements(Cell cell, InlineString inlineString, Text item)
        {
            inlineString.Append(item);
            cell.Append(inlineString);
            _currentlyBuildingRow.Append(cell);
        }
    }
}
