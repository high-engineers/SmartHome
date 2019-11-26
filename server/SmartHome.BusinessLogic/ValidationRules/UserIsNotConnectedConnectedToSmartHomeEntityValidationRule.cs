using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class UserIsNotConnectedConnectedToSmartHomeEntityValidationRule : IValidationRule<UserIsNotConnectedConnectedToSmartHomeEntityValidationRuleData>
    {
        private const string _userIsConnectedToSmartHomeErrorMessage = "UserIsConnectedToSmartHomeError";

        private readonly SmartHomeContext _context;

        public UserIsNotConnectedConnectedToSmartHomeEntityValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(UserIsNotConnectedConnectedToSmartHomeEntityValidationRuleData data)
        {
            var userId = (await _context.Users.FirstOrDefaultAsync(x => x.Email == data.Email)).UserId;
            var result = !(await _context.UserSmartHomeEntities
                .AnyAsync(x => x.SmartHomeEntityId == data.SmartHomeEntityId && x.UserId == userId));

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status403Forbidden, _userIsConnectedToSmartHomeErrorMessage));
        }
    }

    public class UserIsNotConnectedConnectedToSmartHomeEntityValidationRuleData
    {
        public Guid SmartHomeEntityId { get; set; }
        public string Email { get; set; }
    }
}
