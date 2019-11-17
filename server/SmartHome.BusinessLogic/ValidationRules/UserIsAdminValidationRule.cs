using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class UserIsAdminValidationRule : IValidationRule<UserIsAdminValidationRuleData>
    {
        private const string _userIsNotAdminErroMessage = "UserIsNotAdminError";

        private SmartHomeContext _context;

        public UserIsAdminValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(UserIsAdminValidationRuleData data)
        {
            var entity = await
                _context
                .UserSmartHomeEntities
                .FirstOrDefaultAsync(x => x.SmartHomeEntityId == data.SmartHomeEntityId && x.UserId == data.UserId);

            var result = entity.IsAdmin;

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status400BadRequest, _userIsNotAdminErroMessage));
        }

    }
    public class UserIsAdminValidationRuleData
    {
        public Guid UserId { get; set; }
        public Guid SmartHomeEntityId { get; set; }
    }
}
