using System;

namespace SmartHome.BusinessLogic.Components.Models
{
    public class DeviceDetails
    {
        public Guid Id { get; set; }
        public bool IsOn { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
