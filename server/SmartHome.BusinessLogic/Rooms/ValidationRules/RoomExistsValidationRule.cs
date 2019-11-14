using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Rooms.ValidationRules
{
    public class RoomExistsValidationRule : IValidationRule<RoomExistsValidationRuleData>
    {
        private const string _roomDoesntExistErrorMessage = "RoomDoesntExistError";

        private readonly SmartHomeContext _context;

        public RoomExistsValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(RoomExistsValidationRuleData data)
        {
            var result = await _context.Rooms.AnyAsync(x => x.RoomId == data.RoomId);

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status404NotFound, _roomDoesntExistErrorMessage));
        }
    }

    public class RoomExistsValidationRuleData
    {
        public Guid RoomId { get; set; }
    }
}
