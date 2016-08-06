using System.Windows.Input;
using Magisterka.Domain.Adapters;
using Magisterka.ViewModels;
using Magisterka.VisualEcosystem.Animation;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class ClearGraphCommand : RoutedUICommand, ICommand
    {
        private readonly IMovingActor _actor;
        private readonly MainWindowViewModel _applicationWindow;
        private MapAdapter _mapAdapter;

        public ClearGraphCommand(MainWindowViewModel applicationWindow, 
            IMovingActor actor)
            : base("Clear the graph", "ClearGraph", typeof(ClearGraphCommand), new InputGestureCollection
            {
                new KeyGesture(Key.Escape, ModifierKeys.None)
            })
        {
            _applicationWindow = applicationWindow;
            _actor = actor;
        }

        public bool CanExecute(object mapAdapter)
        {
            var adapter = mapAdapter as MapAdapter;
            _mapAdapter = adapter;
            return _mapAdapter != null && _applicationWindow.VisualMap.IsLoaded && _mapAdapter.CanGraphBeCleared();
        }

        public void Execute(object parameter)
        {
            _applicationWindow.VisualMap.ClearGraph();
            _mapAdapter.ClearGraph();
            _applicationWindow.VisualMap.Children.Remove(_actor.PresentActor());
            _applicationWindow.GraphCleared();
        }
    }
}
