using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
        public static IContainer DependencyContainer { get; set; }

        public App()
        {
            DependencyContainer = DependencyProviderConfiguration.ConfigureContainer();
        }

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
