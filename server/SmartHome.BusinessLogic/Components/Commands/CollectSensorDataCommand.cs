using System;

namespace SmartHome.BusinessLogic.Components.Commands
{
    public class CollectSensorDataCommand
    {
        public Guid SmartHomeEntityId { get; set; }
        public Guid RoomId { get; set; }
        public Guid ComponentId { get; set; }
        public decimal Reading { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
