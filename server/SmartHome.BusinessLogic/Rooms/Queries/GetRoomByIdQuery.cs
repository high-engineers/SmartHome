using System;

namespace SmartHome.BusinessLogic.Rooms.Queries
{
    public class GetRoomByIdQuery
    {
        public Guid UserId { get; set; }
        public Guid SmartHomeEntityId { get; set; }
        public Guid RoomId { get; set; }
    }
}
