using System.Collections.Generic;

namespace SmartHome.BusinessLogic.Components.Models
{
    public class SensorHistory
    {
        public IReadOnlyCollection<SensorHistoryEntry> HistoryEntries { get; set; }
        public string Type { get; set; }
    }
}
