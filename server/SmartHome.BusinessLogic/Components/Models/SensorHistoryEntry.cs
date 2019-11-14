using System;

namespace SmartHome.BusinessLogic.Components.Models
{
    public class SensorHistoryEntry
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public decimal Value { get; set; }
    }
}
