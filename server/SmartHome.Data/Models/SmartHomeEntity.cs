using System;
using System.Collections.Generic;

namespace SmartHome.Data.Models
{
    public class SmartHomeEntity
    {
        public Guid SmartHomeEntityId { get; set; }
        public DateTime RegisterTimestamp { get; set; }
        public string Name { get; set; }

        //FKs and nav props
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<UserSmartHomeEntity> UserSmartHomeEntities { get; set; }
        public virtual ICollection<Component> Components { get; set; }
    }
}
