using Microsoft.AspNetCore.Http;
using SmartHome.BusinessLogic.Components.Commands;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using SmartHome.Data.Models;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Components.CommandHandlers
{
    public class CollectSensorDataCommandHandler
    {
        private SmartHomeContext _context;

        private readonly SmartHomeEntityExistsValidationRule _smartHomeEntityExistsValidationRule;
        private readonly RoomExistsValidationRule _roomExistsValidationRule;
        private readonly ComponentExistsValidationRule _componentExistsValidationRule;

        public CollectSensorDataCommandHandler(SmartHomeContext context, SmartHomeEntityExistsValidationRule smartHomeEntityExistsValidationRule, RoomExistsValidationRule roomExistsValidationRule, ComponentExistsValidationRule componentExistsValidationRule)
        {
            _context = context;
            _smartHomeEntityExistsValidationRule = smartHomeEntityExistsValidationRule;
            _roomExistsValidationRule = roomExistsValidationRule;
            _componentExistsValidationRule = componentExistsValidationRule;
        }

        public async Task<IResult<object>> HandleAsync(CollectSensorDataCommand command)
        {
            var validationResult = await IsValidAsync(command);

            if (!validationResult.IsSuccess)
            {
                return Result<object>.Fail(validationResult.ResultError);
            }

            var result = await AddComponentDataAsync(command);
            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status500InternalServerError, "Something went wrong..."));
        }

        private async Task<IResult<object>> IsValidAsync(CollectSensorDataCommand command)
        {
            var resultSmartHomeEntityExists = await _smartHomeEntityExistsValidationRule.ValidateAsync(command.SmartHomeEntityId);

            if (!resultSmartHomeEntityExists.IsSuccess)
            {
                return resultSmartHomeEntityExists;
            }

            var resultRoomExists = await _roomExistsValidationRule.ValidateAsync(new RoomExistsValidationRuleData
            {
                RoomId = command.RoomId
            });

            if (!resultRoomExists.IsSuccess)
            {
                return resultRoomExists;
            }

            var resultComponentExists = await _componentExistsValidationRule.ValidateAsync(command.ComponentId);

            if (!resultComponentExists.IsSuccess)
            {
                return resultComponentExists;
            }

            return Result<object>.Success();
        }

        private async Task<bool> AddComponentDataAsync(CollectSensorDataCommand command)
        {
            try
            {
                await _context.ComponentData.AddAsync(new ComponentData
                {
                    ComponentId = command.ComponentId,
                    Reading = command.Reading,
                    Timestamp = command.Timestamp
                });
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
