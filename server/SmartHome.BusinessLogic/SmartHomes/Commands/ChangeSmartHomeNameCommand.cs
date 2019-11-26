using SmartHome.BusinessLogic.Infrastructure.Models;

namespace SmartHome.BusinessLogic.SmartHomes.Commands
{
    public class ChangeSmartHomeNameCommand : BaseCqrsParam
    {
        public string NewName { get; set; }
    }
}
