using Autofac;

namespace SmartHome.BusinessLogic
{
    public class ModuleBootstraper
    {
        public ModuleBootstraper(ContainerBuilder containerBuilder)
        {
            InitializeQueryHandlers(containerBuilder);
        }

        public void InitializeQueryHandlers(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterAssemblyTypes(GetType().Assembly)
                .Where(x => x.Name.EndsWith("Handler"))
                .AsSelf();
        }
    }
}
