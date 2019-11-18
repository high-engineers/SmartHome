using SmartHome.BusinessLogic.Infrastructure.Models;
using System;

namespace SmartHome.BusinessLogic.Components.Commands
{
    public class AddComponentToRoomCommand : BaseCqrsParam
    {
        public Guid ComponentId { get; set; }
        public Guid RoomId { get; set; }
        public string Name { get; set; }
    }
}
