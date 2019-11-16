using SmartHome.Data.Infrastructure.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SmartHome.Data.Models.Extenstions
{
    public static class ComponentExtensions
    {
        public static Component WhereTypeIs(this ICollection<Component> components, ComponentTypeEnum type)
        {
            return components.FirstOrDefault(x => x.ComponentType.Type == type);
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
