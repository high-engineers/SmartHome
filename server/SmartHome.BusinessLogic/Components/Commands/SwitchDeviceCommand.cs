using SmartHome.BusinessLogic.Infrastructure.Models;
using System;

namespace SmartHome.BusinessLogic.Components.Commands
{
    public class SwitchDeviceCommand : BaseCqrsParam
    {
        public Guid ComponentId { get; set; }
        public bool NewState { get; set; }
    }
}
