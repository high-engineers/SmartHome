using SmartHome.BusinessLogic.Infrastructure.Models;
using System;

namespace SmartHome.BusinessLogic.Rooms.Queries
{
    public class GetRoomsQuery : IRequestedByUser
    {
        public Guid SmartHomeEntityId { get; set; }

        public Guid RequestedByUserId { get; set; }
    }
}
