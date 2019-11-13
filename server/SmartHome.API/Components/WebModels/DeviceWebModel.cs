using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.API.Components.WebModels
{
    public class DeviceWebModel
    {
        public Guid Id { get; set; }
        public bool IsOn { get; set; }
        public string Type { get; set; }
    }
}
