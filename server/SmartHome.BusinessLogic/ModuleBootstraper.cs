using Autofac;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.ValidationRules;
using System;

namespace SmartHome.BusinessLogic
{
    public class ModuleBootstraper
    {
        public ModuleBootstraper(ContainerBuilder containerBuilder)
        {
            InitializeQueryHandlers(containerBuilder);
            InitializeValidationRules(containerBuilder);
        }

        public void InitializeQueryHandlers(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterAssemblyTypes(GetType().Assembly)
                .Where(x => x.Name.EndsWith("Handler"))
                .AsSelf();
        }

        public void InitializeValidationRules(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterAssemblyTypes(GetType().Assembly)
                .Where(x => x.IsClass)
                .Where(x => x.Name.EndsWith("ValidationRule"))
                .AsClosedTypesOf(typeof(IValidationRule<>))
                .InstancePerLifetimeScope();
        }
    }
}
