using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class ComponentHasRoomAssignedValidationRule : IValidationRule<Guid>
    {
        private const string _componentHasNoRoomAssignedErrorMessage = "ComponentHasNoRoomAssignedError";
        private readonly SmartHomeContext _context;

        public ComponentHasRoomAssignedValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(Guid componentId)
        {
            var component = await _context.Components.FirstOrDefaultAsync(x => x.ComponentId == componentId);

            return component.RoomId.HasValue
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status400BadRequest, _componentHasNoRoomAssignedErrorMessage));
        }
    }
}
