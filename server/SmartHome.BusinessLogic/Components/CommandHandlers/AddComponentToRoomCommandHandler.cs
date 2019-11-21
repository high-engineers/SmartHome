using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Components.Commands;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using SmartHome.Data.Models.Extenstions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Components.CommandHandlers
{
    public class AddComponentToRoomCommandHandler
    {
        private readonly SmartHomeContext _context;
        private readonly SmartHomeEntityExistsValidationRule _smartHomeEntityExistsValidationRule;
        private readonly UserExistsValidationRule _userExistsValidationRule;
        private readonly UserIsConnectedToSmartHomeEntityValidationRule _userIsConnectedToSmartHomeEntityValidationRule;
        private readonly UserIsAdminValidationRule _userIsAdminValidationRule;
        private readonly ComponentExistsValidationRule _componentExistsValidationRule;
        private readonly ComponentHasNoRoomAssignedValidationRule _componentHasNoRoomAssignedValidationRule;
        private readonly AddDeviceToRoomColumnValidationRule _addDeviceToRoomColumnValidationRule;
        private readonly DeviceExistsValidationRule _deviceExistsValidationRule;

        public AddComponentToRoomCommandHandler(SmartHomeContext context, SmartHomeEntityExistsValidationRule smartHomeEntityExistsValidationRule, UserExistsValidationRule userExistsValidationRule, UserIsConnectedToSmartHomeEntityValidationRule userIsConnectedToSmartHomeEntityValidationRule, UserIsAdminValidationRule userIsAdminValidationRule, ComponentExistsValidationRule componentExistsValidationRule, ComponentHasNoRoomAssignedValidationRule componentHasNoRoomAssignedValidationRule, AddDeviceToRoomColumnValidationRule addDeviceToRoomColumnValidationRule, DeviceExistsValidationRule deviceExistsValidationRule)
        {
            _context = context;
            _smartHomeEntityExistsValidationRule = smartHomeEntityExistsValidationRule;
            _userExistsValidationRule = userExistsValidationRule;
            _userIsConnectedToSmartHomeEntityValidationRule = userIsConnectedToSmartHomeEntityValidationRule;
            _userIsAdminValidationRule = userIsAdminValidationRule;
            _componentExistsValidationRule = componentExistsValidationRule;
            _componentHasNoRoomAssignedValidationRule = componentHasNoRoomAssignedValidationRule;
            _addDeviceToRoomColumnValidationRule = addDeviceToRoomColumnValidationRule;
            _deviceExistsValidationRule = deviceExistsValidationRule;
        }

        public async Task<IResult<object>> HandleAsync(AddComponentToRoomCommand command)
        {
            var validationResult = await IsValidAsync(command);

            if (!validationResult.IsSuccess)
            {
                return Result<object>.Fail(validationResult.ResultError);
            }

            var result = await AddComponentToRoom(command);
            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status500InternalServerError, "Something went wrong..."));
        }

        private async Task<IResult<object>> IsValidAsync(AddComponentToRoomCommand command)
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

            var resultUserIsAdmin = await _userIsAdminValidationRule.ValidateAsync(new UserIsAdminValidationRuleData
            {
                UserId = command.UserId,
                SmartHomeEntityId = command.SmartHomeEntityId
            });

            if (!resultUserIsAdmin.IsSuccess)
            {
                return resultUserIsAdmin;
            }

            var resultComponentExists = await _componentExistsValidationRule.ValidateAsync(command.ComponentId);

            if (!resultComponentExists.IsSuccess)
            {
                return resultComponentExists;
            }

            var resultComponentHasNoRoomAssigned = await _componentHasNoRoomAssignedValidationRule.ValidateAsync(command.ComponentId);
            if (!resultComponentHasNoRoomAssigned.IsSuccess)
            {
                return resultComponentHasNoRoomAssigned;
            }

            var resultDeviceExists = await _deviceExistsValidationRule.ValidateAsync(command.ComponentId);

            if (resultDeviceExists.IsSuccess)
            {
                var resultColumnValidation = await _addDeviceToRoomColumnValidationRule.ValidateAsync(command.Name);
                if (!resultColumnValidation.IsSuccess)
                {
                    return resultColumnValidation;
                }
            }

            return Result<object>.Success();
        }

        private async Task<bool> AddComponentToRoom(AddComponentToRoomCommand command)
        {
            var component = await _context
                .Components
                .Where(x => x.ComponentId == command.ComponentId)
                .Include(x => x.ComponentType)
                .FirstOrDefaultAsync();
            try
            {
                component.RoomId = command.RoomId;
                if (component.IsDevice())
                {
                    component.Name = command.Name;
                }
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
