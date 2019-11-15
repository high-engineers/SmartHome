using SmartHome.API.Components.WebModels;
using System;
using System.Collections.Generic;

namespace SmartHome.API.Rooms.WebModels
{
    public class RoomWebModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
        public IReadOnlyCollection<DeviceWebModel> Devices { get; set; }
    }
}
