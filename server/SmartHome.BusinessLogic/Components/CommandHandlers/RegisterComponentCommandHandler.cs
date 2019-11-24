using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Components.Commands;
using SmartHome.BusinessLogic.Infrastructure.Extensions;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using SmartHome.Data.Infrastructure.Enums;
using SmartHome.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Components.CommandHandlers
{
    public class RegisterComponentCommandHandler
    {
        private readonly SmartHomeContext _context;

        private readonly SmartHomeEntityExistsValidationRule _smartHomeEntityExistsValidationRule;
        private readonly RoomExistsValidationRule _roomExistsValidationRule;
        private readonly ComponentTypeExistsValidationRule _componentTypeExistsValidationRule;

        public RegisterComponentCommandHandler(SmartHomeContext context, SmartHomeEntityExistsValidationRule smartHomeEntityExistsValidationRule, RoomExistsValidationRule roomExistsValidationRule, ComponentTypeExistsValidationRule componentTypeExistsValidationRule)
        {
            _context = context;
            _smartHomeEntityExistsValidationRule = smartHomeEntityExistsValidationRule;
            _roomExistsValidationRule = roomExistsValidationRule;
            _componentTypeExistsValidationRule = componentTypeExistsValidationRule;
        }

        public async Task<IResult<Guid>> HandleAsync(RegisterComponentCommand command)
        {
            var validationResult = await IsValidAsync(command);

            if (!validationResult.IsSuccess)
            {
                return Result<Guid>.Fail(validationResult.ResultError);
            }

            var componentAdded = await RegisterComponentAsync(command);

            return componentAdded != null
                ? componentAdded.ComponentId.ToSuccessfulResult()
                : Result<Guid>.Fail(new ResultError(StatusCodes.Status500InternalServerError, "Something went wrong..."));
        }

        private async Task<IResult<object>> IsValidAsync(RegisterComponentCommand command)
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

            var resultComponentTypeExists = await _componentTypeExistsValidationRule.ValidateAsync(command.Type);

            if (!resultComponentTypeExists.IsSuccess)
            {
                return resultComponentTypeExists;
            }

            return Result<object>.Success();
        }

        private async Task<Component> RegisterComponentAsync(RegisterComponentCommand command)
        {
            var typeEnum = Enum.Parse<ComponentTypeEnum>(command.Type);

            var componentType = await
                _context
                .ComponentTypes
                .Where(x => x.Type == typeEnum)
                .FirstOrDefaultAsync();

            var newComponent = new Component
            {
                RoomId = command.RoomId,
                ComponentState = ComponentStateEnum.Unknown,
                ComponentTypeId = componentType.ComponentTypeId
            };

            try
            {
                await _context.Components.AddAsync(newComponent);
                await _context.SaveChangesAsync();
                return newComponent;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
