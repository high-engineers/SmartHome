using System;

namespace SmartHome.API.Infrastructure
{
    public class UserIdSmartHomeEntityIdQueryParam
    {
        public Guid RequestedByUserId { get; set; }
        public Guid SmartHomeEntityId { get; set; }
    }
}
