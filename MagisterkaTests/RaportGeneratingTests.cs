using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Core.Internal;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Magisterka;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Monitoring;
using Magisterka.Domain.Monitoring.Performance;
using Magisterka.Domain.Monitoring.Quality;
using Magisterka.Infrastructure.AppSettings;
using Magisterka.Infrastructure.RaportGenerating;
using Magisterka.Infrastructure.RaportGenerating.RaportStaticResources;
using Magisterka.StaticResources;
using MagisterkaTests.TestingStubs;
using Moq;
using NUnit.Framework;

namespace MagisterkaTests
{
    [TestFixture]
    public class RaportGeneratingTests
    {
        private readonly Mock<IAlgorithmMonitor> _monitorMock;
        private readonly IAppSettings _appSettings;

        public RaportGeneratingTests()
        {
            _monitorMock = new Mock<IAlgorithmMonitor>();
            _monitorMock.Setup(x => x.PathDetails).Returns(new PathDetails
            {
                NumberOfVisitsPerNode = new Dictionary<Node, int>
                {
                    { new Node(), 1 },
                    { new Node(), 2 },
                    { new Node(), 1 },
                    { new Node(), 3 },
                    { new Node(), 1 },
                    { new Node(), 0 }
                },
                PathLengthInEdgeCost = 23,
                StepsTaken = 6
            });
            _monitorMock.Setup(x => x.PerformanceResults).Returns(new PerformanceResults
            {
                AverageMemoryUsageInBytes = 2456,
                AverageProcessorUsageInPercents = 70,
                PeakMemoryUsageInBytes = 4522,
                PeakProcessorUsageInPercents = 100,
                TimeOfComputing = TimeSpan.FromSeconds(2)
            });

            _appSettings = new AppSettings();
        }

        [Test]
        public void ShouldGenerateExcelReportFile()
        {
            //Given
            IRaportGenerator raportGenerator = new RaportGenerator(_appSettings);
            IRaportStringContainerContract raportStrings = new RaportStringsStub();
            IRaportCommand raportCommand = new ExcelRaportCommand
            {
                AlgorithmMonitor = _monitorMock.Object,
                PathfindingAlgorithm = ePathfindingAlgorithms.AStar,
                RaportStrings = raportStrings
            };

            //When
            string pathToRaport = raportGenerator.GenerateRaport(raportCommand);

            //Then
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(pathToRaport, false))
            {
                WorkbookPart workbookPart = document.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                bool containsNumberOfSteps =
                    sheetData.InnerText.Contains(_monitorMock.Object.PathDetails.StepsTaken.ToString());
                bool containsAverageMemoryUsage = sheetData.InnerText.Contains(
                    $"{_monitorMock.Object.PerformanceResults.AverageMemoryUsageInBytes/1024}");
                bool containsOneOfTheNodesNames =
                    sheetData.InnerText.Contains(
                        _monitorMock.Object.PathDetails.NumberOfVisitsPerNode.First().Key.Name);
                bool containsNameOfPathfindingAlgorithm =
                    sheetData.InnerText.Contains(raportCommand.PathfindingAlgorithm.ToString());

                Assert.IsTrue(containsNumberOfSteps);
                Assert.IsTrue(containsAverageMemoryUsage);
                Assert.IsTrue(containsOneOfTheNodesNames);
                Assert.IsTrue(containsNameOfPathfindingAlgorithm);
            }
        }

        [TearDown]
        public void TearDown()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(_appSettings.RaportPath);

            foreach (var file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
        }
    }
}
