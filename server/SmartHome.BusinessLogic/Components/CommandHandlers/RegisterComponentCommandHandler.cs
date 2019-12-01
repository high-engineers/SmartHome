using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Components.Commands;
using SmartHome.BusinessLogic.Components.Models;
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

        private readonly ComponentTypeExistsValidationRule _componentTypeExistsValidationRule;
        private readonly SmartHomeEntityExistsValidationRule _smartHomeEntityExistsValidationRule;

        public RegisterComponentCommandHandler(SmartHomeContext context, ComponentTypeExistsValidationRule componentTypeExistsValidationRule, SmartHomeEntityExistsValidationRule smartHomeEntityExistsValidationRule)
        {
            _context = context;
            _componentTypeExistsValidationRule = componentTypeExistsValidationRule;
            _smartHomeEntityExistsValidationRule = smartHomeEntityExistsValidationRule;
        }

        public async Task<IResult<SmartHomeComponent>> HandleAsync(RegisterComponentCommand command)
        {
            var validationResult = await IsValidAsync(command);

            if (!validationResult.IsSuccess)
            {
                return Result<SmartHomeComponent>.Fail(validationResult.ResultError);
            }

            var smartHomeComponent = await RegisterComponentAsync(command);

            return smartHomeComponent != null
                ? smartHomeComponent.ToSuccessfulResult()
                : Result<SmartHomeComponent>.Fail(new ResultError(StatusCodes.Status500InternalServerError, "Something went wrong..."));
        }

        private async Task<IResult<object>> IsValidAsync(RegisterComponentCommand command)
        {
            var resultSmartHomeEntityExists = await _smartHomeEntityExistsValidationRule.ValidateAsync(command.SmartHomeEntityId);

            if (!resultSmartHomeEntityExists.IsSuccess)
            {
                return resultSmartHomeEntityExists;
            }

            var resultComponentTypeExists = await _componentTypeExistsValidationRule.ValidateAsync(command.Type);

            if (!resultComponentTypeExists.IsSuccess)
            {
                return resultComponentTypeExists;
            }

            return Result<object>.Success();
        }

        private async Task<SmartHomeComponent> RegisterComponentAsync(RegisterComponentCommand command)
        {
            var typeEnum = Enum.Parse<ComponentTypeEnum>(command.Type);

            var componentType = await
                _context
                .ComponentTypes
                .Where(x => x.Type == typeEnum)
                .FirstOrDefaultAsync();

            var newComponent = new Component
            {
                ComponentState = ComponentStateEnum.Registered,
                ComponentTypeId = componentType.ComponentTypeId,
                SmartHomeEntityId = command.SmartHomeEntityId
            };

            try
            {
                await _context.Components.AddAsync(newComponent);
                await _context.SaveChangesAsync();
                return new SmartHomeComponent
                {
                    ComponentId = newComponent.ComponentId
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}
