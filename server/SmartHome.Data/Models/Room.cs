﻿using System;
using System.Collections.Generic;

namespace SmartHome.Data.Models
{
    public class Room
    {
        public Guid RoomId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        //FKs and nav props
        public Guid SmartHomeEntityId { get; set; }
        public virtual SmartHomeEntity SmartHomeEntity { get; set; }

        public virtual ICollection<Component> Components { get; set; }
    }
}
