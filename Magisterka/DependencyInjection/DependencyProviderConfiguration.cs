using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Monitoring;
using Magisterka.Domain.Monitoring.Performance;
using Magisterka.Domain.Monitoring.Quality;
using Magisterka.VisualEcosystem.Animation;
using Magisterka.VisualEcosystem.ErrorHandling;
using Magisterka.VisualEcosystem.Validators;

namespace Magisterka.DependencyInjection
{
    public class DependencyProviderConfiguration
    {
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MainWindow>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ErrorDisplayer>().As<IErrorDisplayer>().InstancePerLifetimeScope();
            builder.RegisterType<ConfigurationValidator>().As<IConfigurationValidator>().InstancePerLifetimeScope();
            builder.RegisterType<Random>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<MapFactory>().As<IMapFactory>().InstancePerLifetimeScope();
            builder.RegisterType<PathfinderFactory>().As<IPathfinderFactory>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultActor>().As<IMovingActor>().InstancePerLifetimeScope();
            builder.RegisterType<AlgorithmMonitor>().AsSelf().InstancePerLifetimeScope();

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
