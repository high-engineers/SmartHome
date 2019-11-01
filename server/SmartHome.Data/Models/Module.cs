using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome.Data.Models
{
    public class Module
    {
        public Guid ModuleId { get; set; }
        public bool IsConnected { get; set; }

        //FKs and nav props
        public Guid RoomId { get; set; }
        public virtual Room Room { get; set; }

        public virtual ICollection<Component> Components { get; set; }
    }
}
