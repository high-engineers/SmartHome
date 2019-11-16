using System;

namespace SmartHome.BusinessLogic.Infrastructure.Models
{
    public abstract class BaseCqrsParam
    {
        public Guid UserId { get; set; }
        public Guid SmartHomeEntityId { get; set; }
    }
}
