using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Components.Commands;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using SmartHome.Data.Infrastructure.Enums;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Components.CommandHandlers
{
    public class SwitchDeviceCommandHandler
    {
        private readonly SmartHomeContext _context;
        private readonly SmartHomeEntityExistsValidationRule _smartHomeEntityExistsValidationRule;
        private readonly UserExistsValidationRule _userExistsValidationRule;
        private readonly UserIsConnectedToSmartHomeEntityValidationRule _userIsConnectedToSmartHomeEntityValidationRule;
        private readonly DeviceExistsValidationRule _deviceExistsValidationRule;
        private readonly DeviceIsInRoomValidationRule _deviceIsInRoomValidationRule;

        public SwitchDeviceCommandHandler(SmartHomeContext context, SmartHomeEntityExistsValidationRule smartHomeEntityExistsValidationRule, UserExistsValidationRule userExistsValidationRule, UserIsConnectedToSmartHomeEntityValidationRule userIsConnectedToSmartHomeEntityValidationRule, DeviceExistsValidationRule deviceExistsValidationRule, DeviceIsInRoomValidationRule deviceIsInRoomValidationRule)
        {
            _context = context;
            _smartHomeEntityExistsValidationRule = smartHomeEntityExistsValidationRule;
            _userExistsValidationRule = userExistsValidationRule;
            _userIsConnectedToSmartHomeEntityValidationRule = userIsConnectedToSmartHomeEntityValidationRule;
            _deviceExistsValidationRule = deviceExistsValidationRule;
            _deviceIsInRoomValidationRule = deviceIsInRoomValidationRule;
        }

        public async Task<IResult<object>> HandleAsync(SwitchDeviceCommand command)
        {
            var validationResult = await IsValidAsync(command);

            if (!validationResult.IsSuccess)
            {
                return Result<object>.Fail(validationResult.ResultError);
            }

            return await SwitchDeviceAsync(command) ? Result<object>.Success() : Result<object>.Fail(new ResultError(StatusCodes.Status500InternalServerError, "Something went wrong..."));
        }

        private async Task<IResult<object>> IsValidAsync(SwitchDeviceCommand command)
        {
            var resultSmartHomeEntityExists = await _smartHomeEntityExistsValidationRule.ValidateAsync(command.SmartHomeEntityId);

            if (!resultSmartHomeEntityExists.IsSuccess)
            {
                return resultSmartHomeEntityExists;
            }

            var resultUserExists = await _userExistsValidationRule.ValidateAsync(command.UserId);

            if (!resultUserExists.IsSuccess)
            {
                return resultUserExists;
            }

            var resultUserIsConnectedToSmartHomeEntity = await _userIsConnectedToSmartHomeEntityValidationRule
                .ValidateAsync(new UserIsConnectedToSmartHomeEntityValidationRuleData
                {
                    SmartHomeEntityId = command.SmartHomeEntityId,
                    UserId = command.UserId
                });

            if (!resultUserIsConnectedToSmartHomeEntity.IsSuccess)
            {
                return resultUserIsConnectedToSmartHomeEntity;
            }

            var resultDeviceExists = await _deviceExistsValidationRule.ValidateAsync(command.ComponentId);

            if (!resultDeviceExists.IsSuccess)
            {
                return resultDeviceExists;
            }

            var resultDeviceIsInRoom = await _deviceIsInRoomValidationRule.ValidateAsync(new DeviceIsInRoomValidationRuleData
            {
                DeviceId = command.ComponentId,
                RoomId = command.RoomId
            });

            if (!resultDeviceIsInRoom.IsSuccess)
            {
                return resultDeviceIsInRoom;
            }

            return Result<object>.Success();
        }

        private async Task<bool> SwitchDeviceAsync(SwitchDeviceCommand command)
        {
            var device = await _context.Components.FirstOrDefaultAsync(x => x.ComponentId == command.ComponentId);

            try
            {
                device.ComponentState = command.NewState ? ComponentStateEnum.On : ComponentStateEnum.Off;
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
