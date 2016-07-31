using System.Windows;
using Autofac;
using Magisterka.DependencyInjection;

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

        public void AppStartup(object sender, StartupEventArgs e)
        {
            using (var scope = DependencyContainer.BeginLifetimeScope())
            {
                var mainWindow = scope.Resolve<MainWindow>();
                mainWindow.Show();
            }
        }
    }
}
