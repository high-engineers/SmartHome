﻿using SmartHome.Data.Model.Enums;
using System;
using System.Collections.Generic;

namespace SmartHome.Data.Models
{
    public class Component
    {
        public Guid ComponentId { get; set; }
        public ComponentStateEnum ComponentState { get; set; }

        //FKs and nav props
        public Guid ComponentTypeId { get; set; }
        public virtual ComponentType ComponentType { get; set; }

        public Guid ModuleId { get; set; }
        public virtual Module Module { get; set; }

        public virtual ICollection<ComponentData> ComponentData { get; set; }
    }
}