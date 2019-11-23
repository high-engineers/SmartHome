using System;

namespace SmartHome.BusinessLogic.Components.Queries
{
    public class GetAllComponentsQuery
    {
        public Guid SmartHomeEntityId { get; set; }
        public Guid RoomId { get; set; }
    }
}
