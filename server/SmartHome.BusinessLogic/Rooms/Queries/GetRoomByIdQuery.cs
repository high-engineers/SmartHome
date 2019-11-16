using SmartHome.BusinessLogic.Infrastructure.Models;
using System;

namespace SmartHome.BusinessLogic.Rooms.Queries
{
    public class GetRoomByIdQuery : BaseCqrsParam
    {
        public Guid RoomId { get; set; }
    }
}
