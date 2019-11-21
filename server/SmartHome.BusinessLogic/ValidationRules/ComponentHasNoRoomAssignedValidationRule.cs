using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class ComponentHasNoRoomAssignedValidationRule : IValidationRule<Guid>
    {
        private readonly SmartHomeContext _context;

        private const string _componentHasRoomAssignedAlreadyErrorMessage = "ComponentHasRoomAssignedAlreadyError";
        public ComponentHasNoRoomAssignedValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(Guid componentId)
        {
            var component = await _context.Components.FirstOrDefaultAsync(x => x.ComponentId == componentId);
            var result = component?.RoomId == null;

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status400BadRequest, _componentHasRoomAssignedAlreadyErrorMessage));
        }
    }
}
