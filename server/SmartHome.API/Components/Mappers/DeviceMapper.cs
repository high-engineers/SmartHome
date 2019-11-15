using SmartHome.API.Components.WebModels;
using SmartHome.BusinessLogic.Components.Models;

namespace SmartHome.API.Components.Mappers
{
    public static class DeviceMapper
    {
        public static DeviceWebModel ToWebModel(this Device device)
        {
            return new DeviceWebModel
            {
                Id = device.Id,
                IsOn = device.IsOn,
                Type = device.Type
            };
        }
    }
}
