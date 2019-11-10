using System;

namespace SmartHome.Data.Models
{
    public class UserSmartHomeEntity
    {
        public bool IsAdmin { get; set; }

        //FKs and nav props
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid SmartHomeEntityId { get; set; }
        public virtual SmartHomeEntity SmartHomeEntity { get; set; }
    }
}
