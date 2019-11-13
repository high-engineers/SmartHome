using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;

namespace SmartHome.BusinessLogic.Rooms.ValidationRules
{
    public class UserIsConnectedToSmartHomeEntityValidationRule : IValidationRule<UserIsConnectedToSmartHomeEntityValidationRuleData>
    {
        private const string UserIsNotConnectedToSmartHomeErrorMessage = "UserIsNotConnectedToSmartHomeError";

        private readonly SmartHomeContext _context;

        public UserIsConnectedToSmartHomeEntityValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(UserIsConnectedToSmartHomeEntityValidationRuleData data)
        {
            var result = await _context.UserSmartHomeEntities
                .AnyAsync(x => x.SmartHomeEntityId == data.SmartHomeEntityId && x.UserId == data.UserId);

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status403Forbidden, UserIsNotConnectedToSmartHomeErrorMessage));
        }
    }

    public class UserIsConnectedToSmartHomeEntityValidationRuleData
    {
        public Guid UserId { get; set; }
        public Guid SmartHomeEntityId { get; set; }
    }
}
