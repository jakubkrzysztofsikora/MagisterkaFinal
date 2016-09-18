using Autofac;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Monitoring;
using Magisterka.Domain.Monitoring.Performance;
using Magisterka.Domain.Monitoring.Quality;
using Magisterka.Domain.Utilities;
using Magisterka.Infrastructure.AppSettings;
using Magisterka.Infrastructure.RaportGenerating;
using Magisterka.Infrastructure.RaportGenerating.RaportStaticResources;
using Magisterka.StaticResources;
using Magisterka.ViewModels;
using Magisterka.VisualEcosystem.Animation;
using Magisterka.VisualEcosystem.ErrorHandling;
using Magisterka.VisualEcosystem.Validators;
using Magisterka.VisualEcosystem.WindowCommands;
using MahApps.Metro.Controls.Dialogs;

namespace Magisterka.DependencyInjection
{
    public class DependencyProviderConfiguration
    {
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MainWindow>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<MainWindowViewModel>().AsSelf().WithParameter("dialogCoordinator", DialogCoordinator.Instance).InstancePerLifetimeScope();
            builder.RegisterType<AppSettings>().As<IAppSettings>().InstancePerLifetimeScope();
            builder.RegisterType<ErrorDisplayer>().As<IErrorDisplayer>().InstancePerLifetimeScope();
            builder.RegisterType<ConfigurationValidator>().As<IConfigurationValidator>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultRandomGenerator>().As<IRandomGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<MapFactory>().As<IMapFactory>().InstancePerLifetimeScope();
            builder.RegisterType<PathfinderFactory>().As<IPathfinderFactory>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultActor>().As<IMovingActor>().InstancePerLifetimeScope();
            builder.RegisterType<AlgorithmMonitor>().As<IAlgorithmMonitor>().InstancePerLifetimeScope();
            builder.RegisterType<RaportGenerator>().As<IRaportGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<CommandValidator>().As<ICommandValidator>().InstancePerLifetimeScope();
            builder.RegisterType<UserIconActor>().As<IMovingActor>().InstancePerLifetimeScope();

            builder.RegisterType<RaportStringContainer>()
                .As<IRaportStringContainerContract>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PerformanceMonitor>()
                .As<IPartialMonitor<PerformanceResults>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<AlgorithmQualityRegistry>()
                .As<IBehaviourRegistry<PathDetails>>()
                .InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
