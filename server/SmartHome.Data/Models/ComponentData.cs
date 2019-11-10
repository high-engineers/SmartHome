using System;

namespace SmartHome.Data.Models
{
    public class ComponentData
    {
        public Guid ComponentDataId { get; set; }
        public decimal Reading { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        //FKs and nav props
        public Guid ComponentId { get; set; }
        public virtual Component Component { get; set; }
    }
}
