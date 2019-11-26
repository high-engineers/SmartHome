using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.SmartHomes.Commands;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.SmartHomes.CommandHandlers
{
    public class ChangeSmartHomeNameCommandHandler
    {
        private readonly SmartHomeContext _context;

        private readonly SmartHomeEntityExistsValidationRule _smartHomeEntityExistsValidationRule;
        private readonly UserExistsValidationRule _userExistsValidationRule;
        private readonly UserIsConnectedToSmartHomeEntityValidationRule _userIsConnectedToSmartHomeEntityValidationRule;
        private readonly UserIsAdminValidationRule _userIsAdminValidationRule;
        private readonly ChangeSmartHomeNameColumnValidationRule _changeSmartHomeNameColumnValidationRule;

        public ChangeSmartHomeNameCommandHandler(SmartHomeContext context, SmartHomeEntityExistsValidationRule smartHomeEntityExistsValidationRule, UserExistsValidationRule userExistsValidationRule, UserIsConnectedToSmartHomeEntityValidationRule userIsConnectedToSmartHomeEntityValidationRule, UserIsAdminValidationRule userIsAdminValidationRule, ChangeSmartHomeNameColumnValidationRule changeSmartHomeNameColumnValidationRule)
        {
            _context = context;
            _smartHomeEntityExistsValidationRule = smartHomeEntityExistsValidationRule;
            _userExistsValidationRule = userExistsValidationRule;
            _userIsConnectedToSmartHomeEntityValidationRule = userIsConnectedToSmartHomeEntityValidationRule;
            _userIsAdminValidationRule = userIsAdminValidationRule;
            _changeSmartHomeNameColumnValidationRule = changeSmartHomeNameColumnValidationRule;
        }

        public async Task<IResult<object>> HandleAsync(ChangeSmartHomeNameCommand command)
        {
            var validationResult = await IsValidAsync(command);

            if (!validationResult.IsSuccess)
            {
                return Result<object>.Fail(validationResult.ResultError);
            }

            var result = await ChangeSmartHomeNameAsync(command);
            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status500InternalServerError, "Something went wrong..."));
        }

        private async Task<IResult<object>> IsValidAsync(ChangeSmartHomeNameCommand command)
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

            var resultIsAdmin = await _userIsAdminValidationRule.ValidateAsync(new UserIsAdminValidationRuleData
            {
                UserId = command.UserId,
                SmartHomeEntityId = command.SmartHomeEntityId
            });

            if (!resultIsAdmin.IsSuccess)
            {
                return resultIsAdmin;
            }

            var resultColumnValidation = await _changeSmartHomeNameColumnValidationRule.ValidateAsync(command.NewName);

            if (!resultColumnValidation.IsSuccess)
            {
                return resultColumnValidation;
            }

            return Result<object>.Success();
        }

        private async Task<bool> ChangeSmartHomeNameAsync(ChangeSmartHomeNameCommand command)
        {
            var smartHome = await _context.SmartHomeEntities.FirstOrDefaultAsync(x => x.SmartHomeEntityId == command.SmartHomeEntityId);
            smartHome.Name = command.NewName;

            try
            {
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
