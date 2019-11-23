using System;

namespace SmartHome.API.Infrastructure
{
    public class SmartHomeEntityIdRoomIdQueryParam
    {
        public Guid SmartHomeEntityId { get; set; }
        public Guid RoomId { get; set; }
    }
}
