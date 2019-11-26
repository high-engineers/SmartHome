using System;

namespace SmartHome.API.Infrastructure.PutModels
{
    public class SetAdminModel
    {
        public Guid UserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
