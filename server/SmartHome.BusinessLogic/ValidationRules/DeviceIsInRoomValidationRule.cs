using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class DeviceIsInRoomValidationRule : IValidationRule<DeviceIsInRoomValidationRuleData>
    {
        private const string _deviceIsNotInRoomErrorMessage = "DeviceIsNotInRoomError";

        private SmartHomeContext _context;

        public DeviceIsInRoomValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(DeviceIsInRoomValidationRuleData data)
        {
            var result = await _context.Components.AnyAsync(x => x.ComponentId == data.DeviceId && x.RoomId == data.RoomId);

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status400BadRequest, _deviceIsNotInRoomErrorMessage));
        }
    }

    public class DeviceIsInRoomValidationRuleData
    {
        public Guid DeviceId { get; set; }
        public Guid RoomId { get; set; }
    }
}
