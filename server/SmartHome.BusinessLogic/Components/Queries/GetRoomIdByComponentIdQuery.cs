using System;

namespace SmartHome.BusinessLogic.Components.Queries
{
    public class GetRoomIdByComponentIdQuery
    {
        public Guid ComponentId { get; set; }
        public Guid SmartHomeEntityId { get; set; }
    }
}
