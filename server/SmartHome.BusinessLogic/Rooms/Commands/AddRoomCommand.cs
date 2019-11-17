using SmartHome.BusinessLogic.Infrastructure.Models;

namespace SmartHome.BusinessLogic.Rooms.Commands
{
    public class AddRoomCommand : BaseCqrsParam
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
