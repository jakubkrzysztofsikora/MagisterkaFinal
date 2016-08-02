using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FontAwesome.WPF;
using Magisterka.ViewModels;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class ToggleEdgeArrowsCommand : RoutedUICommand, ICommand
    {
        private readonly MainWindowViewModel _window;

        public ToggleEdgeArrowsCommand(MainWindowViewModel window)
        {
            _window = window;
        }

        public bool CanExecute(object parameter)
        {
            return _window.VisualMap != null && _window.VisualMap.IsLoaded && _window.VisualMap.IsInitialized && _window.VisualMap.LogicCore != null;
        }

        public void Execute(object parameter)
        {
            _window.VisualMap.ShowAllEdgesArrows(!_window.VisualMap.ShowEdgeArrows);
            _window.SetDefaultIcons();
        }
    }
}
