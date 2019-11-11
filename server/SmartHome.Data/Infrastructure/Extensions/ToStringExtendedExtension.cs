using SmartHome.Data.Model.Enums;

namespace SmartHome.Data.Infrastructure.Extensions
{
    public static class ToStringExtendedExtension
    {
        public static string ToStringExtended(this ComponentTypeEnum componentType)
        {
            switch (componentType)
            {
                case ComponentTypeEnum.LightBulb:
                    return "Light bulb";
                case ComponentTypeEnum.LightSensor:
                    return "Light sensor";
                case ComponentTypeEnum.Switch:
                    return "Switch";
                case ComponentTypeEnum.Thermometer:
                    return "Thermometer";
                case ComponentTypeEnum.MotionSensor:
                    return "Motion sensor";
                default:
                    return "Unknown";
            }
        }

        public static string ToStringExtended(this ComponentStateEnum componentState)
        {
            switch (componentState)
            {
                case ComponentStateEnum.On:
                    return "ON";
                case ComponentStateEnum.Off:
                    return "OFF";
                case ComponentStateEnum.Collecting:
                    return "Collecting";
                case ComponentStateEnum.Unknown:
                default:
                    return "Unknown";
            }
        }
    }
}
