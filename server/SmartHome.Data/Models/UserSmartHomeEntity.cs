using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome.Data.Models
{
    public class UserSmartHomeEntity
    {

        //FKs and nav props
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid SmartHomeEntityId { get; set; }
        public virtual SmartHomeEntity SmartHomeEntity { get; set; }
    }
}
