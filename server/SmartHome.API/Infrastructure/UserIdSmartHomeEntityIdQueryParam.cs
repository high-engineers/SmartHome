using System;

namespace SmartHome.API.Infrastructure
{
    public class UserIdSmartHomeEntityIdQueryParam
    {
        public Guid UserId { get; set; }
        public Guid SmartHomeEntityId { get; set; }
    }
}
