using System;

namespace SmartHome.BusinessLogic.Infrastructure.Models
{
    internal interface IRequestedByUser
    {
        Guid RequestedByUserId { get; set; }
    }
}
