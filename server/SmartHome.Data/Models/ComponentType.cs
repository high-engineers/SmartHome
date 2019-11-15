using SmartHome.Data.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace SmartHome.Data.Models
{
    public class ComponentType
    {
        public Guid ComponentTypeId { get; set; }
        public ComponentTypeEnum Type { get; set; }
        public bool IsSwitchable { get; set; }

        //FKs and nav props
        public virtual ICollection<Component> Components{ get; set; }
    }
}
