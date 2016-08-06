using System.Windows;
using GraphX.Controls;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.ViewModels;
using Magisterka.ViewModels;
using Magisterka.VisualEcosystem.Extensions;
using Magisterka.VisualEcosystem.InputModals;

namespace Magisterka.VisualEcosystem
{
    public class EdgeCostChangeProcessor
    {
        public int NewEdgeCost { get; private set; }
        private readonly EdgeView _edgeViewModel;
        private readonly EdgeAdapter _edgeAdapter;
        private readonly MainWindowViewModel _mailMainWindowViewModel;

        public EdgeCostChangeProcessor(EdgeControl edgeControl, MainWindowViewModel mainWindowViewModel)
        {
            _edgeViewModel = edgeControl.GetEdgeView();
            _mailMainWindowViewModel = mainWindowViewModel;
        }

        public EdgeCostChangeProcessor(EdgeView edgeViewModel, MainWindowViewModel mainWindowViewModel)
        {
            _edgeViewModel = edgeViewModel;
            _mailMainWindowViewModel = mainWindowViewModel;
        }

        public EdgeCostChangeProcessor(EdgeAdapter edgeAdapter, MainWindowViewModel mainWindowViewModel)
        {
            _edgeAdapter = edgeAdapter;
            _mailMainWindowViewModel = mainWindowViewModel;
        }

        public bool ChangeEdgeCost()
        {
            if (_edgeViewModel == null)
                return AddEdgeCost();

            ChangeEdgeCostModal modal = new ChangeEdgeCostModal($"{_edgeViewModel.Caption} {Application.Current.Resources["ChangeEdgeCost"]}", _edgeViewModel.LogicEdge.Cost);
            bool? answered = modal.ShowDialog();

            if (answered == null || !answered.Value)
                return false;

            _mailMainWindowViewModel.MapAdapter.ChangeCost(_edgeViewModel, modal.Answer);

            NewEdgeCost = modal.Answer;
            return true;
        }

        private bool AddEdgeCost()
        {
            ChangeEdgeCostModal modal = new ChangeEdgeCostModal($"{Application.Current.Resources["NewEdgeCostTitle"]}", _edgeAdapter.Edge.Cost);
            bool? answered = modal.ShowDialog();

            if (answered == null || answered.Value == false)
                return false;

            NewEdgeCost = modal.Answer;
            return true;
        }
    }
}
