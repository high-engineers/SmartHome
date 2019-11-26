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

        public RegisterComponentCommandHandler(SmartHomeContext context, ComponentTypeExistsValidationRule componentTypeExistsValidationRule)
        {
            _context = context;
            _componentTypeExistsValidationRule = componentTypeExistsValidationRule;
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
                ComponentTypeId = componentType.ComponentTypeId
            };

            try
            {
                var smartHomeEntity = await _context.SmartHomeEntities.FirstOrDefaultAsync(x => x.IpAddress == command.IpAddress);
                Guid smartHomeEntityId;

                //SmartHome does not exist. Need to create one.
                if (smartHomeEntity == null)
                {
                    var newSmartHomeEntity = new SmartHomeEntity
                    {
                        IpAddress = command.IpAddress,
                        RegisterTimestamp = DateTime.Now,
                    };

                    await _context.SmartHomeEntities.AddAsync(newSmartHomeEntity);
                    smartHomeEntityId = newSmartHomeEntity.SmartHomeEntityId;
                }
                else
                {
                    smartHomeEntityId = smartHomeEntity.SmartHomeEntityId;
                }

                await _context.Components.AddAsync(newComponent);
                await _context.SaveChangesAsync();
                return new SmartHomeComponent
                {
                    ComponentId = newComponent.ComponentId,
                    SmartHomeEntityId = smartHomeEntityId
                };
            }
            catch(Exception e)
            {
                return null;
            }
        }

    }
}
