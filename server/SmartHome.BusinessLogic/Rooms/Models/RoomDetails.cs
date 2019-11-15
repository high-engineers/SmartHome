using SmartHome.BusinessLogic.Components.Models;
using System;
using System.Collections.Generic;

namespace SmartHome.BusinessLogic.Rooms.Models
{
    public class RoomDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IReadOnlyCollection<SensorHistory> SensorsHistory { get; set; }
        public IReadOnlyCollection<DeviceDetails> Devices { get; set; }
    }
}
