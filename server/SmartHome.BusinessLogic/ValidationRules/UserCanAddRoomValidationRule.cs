using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class UserCanAddRoomValidationRule : IValidationRule<UserCanAddRoomValidationRuleData>
    {
        private const string _userCannotAddRoomErrorMessage = "UserCannotAddRoomError";

        private SmartHomeContext _context;

        public UserCanAddRoomValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(UserCanAddRoomValidationRuleData data)
        {
            var entity = await
                _context
                .UserSmartHomeEntities
                .FirstOrDefaultAsync(x => x.SmartHomeEntityId == data.SmartHomeEntityId && x.UserId == data.UserId);

            var result = entity.IsAdmin;

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status400BadRequest, _userCannotAddRoomErrorMessage));
        }

    }
    public class UserCanAddRoomValidationRuleData
    {
        public Guid UserId { get; set; }
        public Guid SmartHomeEntityId { get; set; }
    }
}
