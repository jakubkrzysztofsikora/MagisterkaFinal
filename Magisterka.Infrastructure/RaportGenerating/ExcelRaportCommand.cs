using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Monitoring;
using Magisterka.Infrastructure.RaportGenerating.ExcelUtilities;
using Magisterka.Infrastructure.RaportGenerating.RaportStaticResources;
using static DocumentFormat.OpenXml.Packaging.SpreadsheetDocument;

namespace Magisterka.Infrastructure.RaportGenerating
{
    public class ExcelRaportCommand : IRaportCommand
    {
        private const string SheetId = "rId1";
        private const string NamespacePrefix = "r";
        private const string NamespaceAddress = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        public ePathfindingAlgorithms PathfindingAlgorithm { get; set; }
        public IAlgorithmMonitor AlgorithmMonitor { get; set; }
        public IRaportStringContainerContract RaportStrings { get; set; }

        public string CreateRaportFile(string path)
        {
            var filePath = Path.Combine(path, CreateExcelName());

            using (SpreadsheetDocument document = Create(filePath,
                    SpreadsheetDocumentType.Workbook))
            {
                WorksheetPart worksheetPart1 = CreateNewWorkbook(document)
                    .AddNewPart<WorksheetPart>(SheetId);

                worksheetPart1.Worksheet = BuildRaportWorksheet();
            }

            return Path.GetFullPath(filePath);
        }

        private Worksheet BuildRaportWorksheet()
        {
            ExcelWorksheetBuilder worksheetBuilder = ExcelWorksheetBuilder.StartCreation()
                    .WithRow($"{PathfindingAlgorithm} {RaportStrings.RaportHeaderTitle}", $"{RaportStrings.RaportHeaderDate}: {DateTime.Now}")
                    .WithSpacingRow()
                    .WithRow(RaportStrings.RaportPathDetailsSectionTitle, RaportStrings.RaportNumberOfSteps, RaportStrings.RaportLengthOfPath)
                    .WithRow(string.Empty, AlgorithmMonitor.PathDetails.StepsTaken, AlgorithmMonitor.PathDetails.PathLengthInEdgeCost)
                    .WithSpacingRow()
                    .WithRow(RaportStrings.RaportVisitsPerNodeSectionTitle, RaportStrings.RaportNodeName, RaportStrings.RaportNodeVisits);

            worksheetBuilder = AlgorithmMonitor.PathDetails.NumberOfVisitsPerNode.Aggregate(worksheetBuilder, (current, positionVisitsPair) => current.WithRow(string.Empty, positionVisitsPair.Key.Name, positionVisitsPair.Value));

            worksheetBuilder = worksheetBuilder
                .WithSpacingRow()
                .WithRow(RaportStrings.RaportPerformanceResultsSectionTitle, RaportStrings.RaportTimeOfComputing, RaportStrings.RaportPeakMemoryUsage,
                    RaportStrings.RaportAverageMemoryUsage, RaportStrings.RaportPeakProcessorUsage, RaportStrings.RaportAverageProcessorUsage)
                .WithRow(string.Empty, AlgorithmMonitor.PerformanceResults.TimeOfComputing.Milliseconds,
                    AlgorithmMonitor.PerformanceResults.PeakMemoryUsageInBytes / 1024,
                    AlgorithmMonitor.PerformanceResults.AverageMemoryUsageInBytes / 1024,
                    AlgorithmMonitor.PerformanceResults.PeakProcessorUsageInPercents,
                    AlgorithmMonitor.PerformanceResults.AverageProcessorUsageInPercents);

            return worksheetBuilder.Build();
        }

        private WorkbookPart CreateNewWorkbook(SpreadsheetDocument document)
        {
            WorkbookPart workbookPart1 = document.AddWorkbookPart();
            Workbook workbook = new Workbook();
            workbook.AddNamespaceDeclaration(NamespacePrefix, NamespaceAddress);

            Sheets sheets = new Sheets();
            Sheet resultsSheet = new Sheet
            {
                Name = $"{PathfindingAlgorithm} {RaportStrings.RaportHeaderTitle}",
                SheetId = 1U,
                Id = SheetId
            };

            sheets.Append(resultsSheet);
            workbook.Append(sheets);
            workbookPart1.Workbook = workbook;

            return workbookPart1;
        }

        private string CreateExcelName()
        {
            return $"Raport-{PathfindingAlgorithm}-{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}.xlsx";
        }
    }
}
