using SmartHome.BusinessLogic.Components.Models;
using System;
using System.Collections.Generic;

namespace SmartHome.BusinessLogic.Rooms.Models
{
    public class RoomBasicInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public IReadOnlyCollection<Device> Devices { get; set; }

    }
}
