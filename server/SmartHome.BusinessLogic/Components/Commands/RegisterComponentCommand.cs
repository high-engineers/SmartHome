using System;

namespace SmartHome.BusinessLogic.Components.Commands
{
    public class RegisterComponentCommand
    {
        public Guid SmartHomeEntityId { get; set; }
        public string Type { get; set; }
    }
}
