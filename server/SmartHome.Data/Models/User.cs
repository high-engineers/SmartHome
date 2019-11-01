using System;
using System.Collections.Generic;

namespace SmartHome.Data.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }

        //FKs and nav props
        public virtual ICollection<UserSmartHomeEntity> UserSmartHomeEntities { get; set; }
    }
}
