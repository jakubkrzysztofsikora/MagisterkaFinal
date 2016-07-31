using System;
using System.Windows;
using System.Windows.Input;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.InputModals;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class AddNewEdgeCommand : RoutedUICommand, ICommand
    {
        private EdgeAdapter _edgeAdapter;
        private readonly MainWindow _window;

        public AddNewEdgeCommand(MainWindow window)
        {
            _window = window;
        }

        public bool CanExecute(object edgeAdapter)
        {
            var adapter = edgeAdapter as EdgeAdapter;
            _edgeAdapter = adapter;

            return _edgeAdapter?.MapAdapter != null && _window.IsLoaded;
        }

        public void Execute(object edgeAdapter)
        {
            ChangeEdgeCostModal modal = new ChangeEdgeCostModal($"{Application.Current.Resources["NewEdgeCostTitle"]}", _edgeAdapter.Edge.Cost);
            bool? answered = modal.ShowDialog();

            if (answered != null && answered.Value == true)
                _edgeAdapter.Edge.Cost = modal.Answer;

            EdgeAdapter mirroredEdgeAdapter = _edgeAdapter.GetEdgeAdapterWithMirroredEdges();
            AddEdge(_edgeAdapter);
            AddEdge(mirroredEdgeAdapter);
            _window.VisualMap.RefreshGraph();
        }

        private void AddEdge(EdgeAdapter edgeAdapter)
        {
            edgeAdapter.MapAdapter.AddEdge(new EdgeView(edgeAdapter.Edge,
                edgeAdapter.MapAdapter.VisualMap.GetVertexByLogicNode(edgeAdapter.FromNode),
                edgeAdapter.MapAdapter.VisualMap.GetVertexByLogicNode(edgeAdapter.ToNode)));
        }
    }
}
