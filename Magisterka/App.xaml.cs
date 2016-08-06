using System;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using Magisterka.DependencyInjection;
using Magisterka.Domain.ExceptionContracts;
using Magisterka.VisualEcosystem.ErrorHandling;

namespace Magisterka
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DependencyContainer = DependencyProviderConfiguration.ConfigureContainer();
        }

        public static IContainer DependencyContainer { get; set; }
        private IErrorDisplayer _errorDisplayer;

        public void AppStartup(object sender, StartupEventArgs e)
        {
            using (var scope = DependencyContainer.BeginLifetimeScope())
            {
                var mainWindow = scope.Resolve<MainWindow>();
                _errorDisplayer = scope.Resolve<IErrorDisplayer>();
                mainWindow.Show();
                Current.DispatcherUnhandledException += CurrentDomainOnUnhandledException;
            }
        }

        private void CurrentDomainOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            var exception = dispatcherUnhandledExceptionEventArgs.Exception;
            _errorDisplayer.DisplayError(eErrorTypes.General, $"Sorry. Unexpected error occured: {exception.Message}");
            dispatcherUnhandledExceptionEventArgs.Handled = true;
        }
    }
}
