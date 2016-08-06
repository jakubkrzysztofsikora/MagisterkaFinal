using System.Windows.Input;
using GraphX.Controls;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.ViewModels;
using Magisterka.ViewModels;
using Magisterka.VisualEcosystem.Extensions;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class AddNewEdgeCommand : RoutedUICommand, ICommand
    {
        private EdgeAdapter _edgeAdapter;
        private readonly MainWindowViewModel _window;

        public AddNewEdgeCommand(MainWindowViewModel window)
        {
            _window = window;
        }

        public bool CanExecute(object edgeAdapter)
        {
            var adapter = edgeAdapter as EdgeAdapter;
            _edgeAdapter = adapter;

            return _edgeAdapter?.MapAdapter != null && _window.VisualMap.IsLoaded;
        }

        public void Execute(object edgeAdapter)
        {
            var edgeCostProcessor = new EdgeCostChangeProcessor(_edgeAdapter, _window);
            bool changedCost = edgeCostProcessor.ChangeEdgeCost();

            if (!changedCost)
                return;

            _edgeAdapter.Edge.Cost = edgeCostProcessor.NewEdgeCost;
            EdgeAdapter mirroredEdgeAdapter = _edgeAdapter.GetEdgeAdapterWithMirroredEdges();
            AddEdge(_edgeAdapter);
            AddEdge(mirroredEdgeAdapter);
            _window.VisualMap.RefreshGraph();
        }

        private void AddEdge(EdgeAdapter edgeAdapter)
        {
            EdgeView edgeView = new EdgeView(edgeAdapter.Edge,
                edgeAdapter.MapAdapter.VisualMap.GetVertexByLogicNode(edgeAdapter.FromNode),
                edgeAdapter.MapAdapter.VisualMap.GetVertexByLogicNode(edgeAdapter.ToNode));
            edgeAdapter.MapAdapter.AddEdge(edgeView);
            VertexControl fromVertexControl = _window.VisualMap.GetVertexControlOfNode(edgeView.Source);
            VertexControl toVertexControl = _window.VisualMap.GetVertexControlOfNode(edgeView.Target);
            _window.VisualMap.AddEdge(edgeView, fromVertexControl, toVertexControl);
        }
    }
}
