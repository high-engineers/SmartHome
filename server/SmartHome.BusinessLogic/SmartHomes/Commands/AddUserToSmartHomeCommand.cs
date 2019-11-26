using SmartHome.BusinessLogic.Infrastructure.Models;

namespace SmartHome.BusinessLogic.SmartHomes.Commands
{
    public class AddUserToSmartHomeCommand : BaseCqrsParam
    {
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
