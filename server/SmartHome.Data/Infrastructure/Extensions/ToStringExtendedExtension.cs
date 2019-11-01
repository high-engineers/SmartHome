using SmartHome.Data.Model.Enums;

namespace SmartHome.Data.Infrastructure.Extensions
{
    public static class ToStringExtendedExtension
    {
        public static string ToStringExtended(this ComponentTypeEnum componentType)
        {
            switch(componentType)
            {
                case ComponentTypeEnum.LIGHT_BULB:
                    return "Light bulb";
                case ComponentTypeEnum.LIGHT_SENSOR:
                    return "Light sensor";
                case ComponentTypeEnum.SWITCH:
                    return "Switch";
                case ComponentTypeEnum.THERMOMETER:
                    return "Thermometer";
                default:
                    return "Unknown";
            }
        }

        public static string ToStringExtended(this ComponentStateEnum componentState)
        {
            switch(componentState)
            {
                case ComponentStateEnum.ON:
                    return "ON";
                case ComponentStateEnum.OFF:
                    return "OFF";
                case ComponentStateEnum.COLLECTING:
                    return "Collecting";
                case ComponentStateEnum.UNKNOWN:
                default:
                    return "Unknown";
            }
        }


    }

    
}
