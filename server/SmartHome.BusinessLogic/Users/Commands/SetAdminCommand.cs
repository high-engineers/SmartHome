using SmartHome.BusinessLogic.Infrastructure.Models;
using System;

namespace SmartHome.BusinessLogic.Users.Commands
{
    public class SetAdminCommand : BaseCqrsParam
    {
        public Guid UserIdToSetAdmin { get; set; }
        public bool IsAdmin { get; set; }
    }
}
