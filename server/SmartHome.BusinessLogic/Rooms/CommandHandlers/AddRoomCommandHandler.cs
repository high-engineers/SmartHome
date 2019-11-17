using Microsoft.AspNetCore.Http;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.Rooms.Commands;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using SmartHome.Data.Models;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Rooms.CommandHandlers
{
    public class AddRoomCommandHandler
    {
        private readonly SmartHomeContext _context;
        private readonly SmartHomeEntityExistsValidationRule _smartHomeEntityExistsValidationRule;
        private readonly UserExistsValidationRule _userExistsValidationRule;
        private readonly UserIsConnectedToSmartHomeEntityValidationRule _userIsConnectedToSmartHomeEntityValidationRule;
        private readonly AddRoomColumnValidationRule _addRoomColumnValidationRule;
        private readonly UserIsAdminValidationRule _userCanAddRoomValidationRule;

        public AddRoomCommandHandler(SmartHomeContext context, SmartHomeEntityExistsValidationRule smartHomeEntityExistsValidationRule, UserExistsValidationRule userExistsValidationRule, UserIsConnectedToSmartHomeEntityValidationRule userIsConnectedToSmartHomeEntityValidationRule, AddRoomColumnValidationRule addRoomColumnValidationRule, UserIsAdminValidationRule userCanAddRoomValidationRule)
        {
            _context = context;
            _smartHomeEntityExistsValidationRule = smartHomeEntityExistsValidationRule;
            _userExistsValidationRule = userExistsValidationRule;
            _userIsConnectedToSmartHomeEntityValidationRule = userIsConnectedToSmartHomeEntityValidationRule;
            _addRoomColumnValidationRule = addRoomColumnValidationRule;
            _userCanAddRoomValidationRule = userCanAddRoomValidationRule;
        }

        private async Task<IResult<object>> IsValidAsync(AddRoomCommand command)
        {
            var resultColumnValidation = await _addRoomColumnValidationRule.ValidateAsync(new AddRoomColumnValidationRuleData
            {
                Name = command.Name,
                Type = command.Type
            });

            if (!resultColumnValidation.IsSuccess)
            {
                return resultColumnValidation;
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

            var resultUserCanAddRoom = await _userCanAddRoomValidationRule.ValidateAsync(new UserIsAdminValidationRuleData
            {
                UserId = command.UserId,
                SmartHomeEntityId = command.SmartHomeEntityId
            });

            if (!resultUserCanAddRoom.IsSuccess)
            {
                return resultUserCanAddRoom;
            }

            return Result<object>.Success();
        }

        public async Task<IResult<object>> HandleAsync(AddRoomCommand command)
        {
            var validationResult = await IsValidAsync(command);

            if (!validationResult.IsSuccess)
            {
                return Result<object>.Fail(validationResult.ResultError);
            }

            return await AddRoomAsync(command)
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status500InternalServerError, "Something went wrong..."));
        }

        private async Task<bool> AddRoomAsync(AddRoomCommand command)
        {
            var newRoom = new Room
            {
                Name = command.Name,
                Type = command.Type,
                SmartHomeEntityId = command.SmartHomeEntityId
            };

            try
            {
                _context.Rooms.Add(newRoom);
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
