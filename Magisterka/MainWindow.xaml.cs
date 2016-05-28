using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphX.Controls;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using GraphX.PCL.Logic.Models;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem;
using QuickGraph;

namespace Magisterka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MapFactory mapFactory = new MapFactory(new Random());
            Map map = mapFactory.GenerateDefaultMap().WithGridPositions();
            MapAdapter adapter = new MapAdapter(map);
            var logicCore = new GXLogicCore<NodeView, EdgeView, BidirectionalGraph<NodeView, EdgeView>> { Graph = adapter.VisualMap };
            
            VisualMap.LogicCore = logicCore;

            VisualMap.GenerateGraph();
            ZoomControl.ZoomToFill();
        }
    }
}
