using SmartHome.Data.Infrastructure.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SmartHome.Data.Models.Extenstions
{
    public static class ComponentExtensions
    {
        public static Component WhereTypeIs(this ICollection<Component> components, ComponentTypeEnum type)
        {
            return components.FirstOrDefault(x => x.ComponentType.Type == type);
        }

        public static bool IsTypeIn(this Component component, ICollection<ComponentTypeEnum> types)
        {
            return types.Contains(component.ComponentType.Type);
        }

        public static bool IsDevice(this Component component)
        {
            return component.IsTypeIn(new Collection<ComponentTypeEnum> { ComponentTypeEnum.LightBulb, ComponentTypeEnum.MotionSensor });
        }

        public static decimal? GetCurrentTemperature(this ICollection<Component> components)
        {
            return 
                components
                .WhereTypeIs(ComponentTypeEnum.Thermometer)
                ?.ComponentData
                .OrderByDescending(x => x.Timestamp)
                .FirstOrDefault()?.Reading;
        }

        public static decimal? GetCurrentHumidity(this ICollection<Component> components)
        {
            return 
                components
                .WhereTypeIs(ComponentTypeEnum.HumiditySensor)
                ?.ComponentData.OrderByDescending(x => x.Timestamp)
                .FirstOrDefault()
                ?.Reading;
        }
    }
}
