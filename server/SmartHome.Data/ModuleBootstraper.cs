using Autofac;

namespace SmartHome.Data
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
                .RegisterType<SmartHomeContext>()
                .AsSelf();
        }
    }
}
