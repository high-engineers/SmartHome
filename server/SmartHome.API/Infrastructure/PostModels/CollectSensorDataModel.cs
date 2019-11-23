using System;

namespace SmartHome.API.Infrastructure.PostModels
{
    public class CollectSensorDataModel
    { 
        public Guid SensorId { get; set; }
        public decimal Reading { get; set; }
    }
}
