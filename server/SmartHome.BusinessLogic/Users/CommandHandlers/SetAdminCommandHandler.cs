using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.Users.Commands;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Users.CommandHandlers
{
    public class SetAdminCommandHandler
    {
        private readonly SmartHomeContext _context;
        private readonly SmartHomeEntityExistsValidationRule _smartHomeEntityExistsValidationRule;
        private readonly UserExistsValidationRule _userExistsValidationRule;
        private readonly UserIsConnectedToSmartHomeEntityValidationRule _userIsConnectedToSmartHomeEntityValidationRule;
        private readonly UserIsAdminValidationRule _userIsAdminValidationRule;

        public SetAdminCommandHandler(SmartHomeContext context, SmartHomeEntityExistsValidationRule smartHomeEntityExistsValidationRule, UserExistsValidationRule userExistsValidationRule, UserIsConnectedToSmartHomeEntityValidationRule userIsConnectedToSmartHomeEntityValidationRule, UserIsAdminValidationRule userIsAdminValidationRule)
        {
            _context = context;
            _smartHomeEntityExistsValidationRule = smartHomeEntityExistsValidationRule;
            _userExistsValidationRule = userExistsValidationRule;
            _userIsConnectedToSmartHomeEntityValidationRule = userIsConnectedToSmartHomeEntityValidationRule;
            _userIsAdminValidationRule = userIsAdminValidationRule;
        }

        public async Task<IResult<object>> HandleAsync(SetAdminCommand command)
        {
            var validationResult = await IsValidAsync(command);

            if (!validationResult.IsSuccess)
            {
                return Result<object>.Fail(validationResult.ResultError);
            }
            var result = await SetAdminForUser(command);

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status500InternalServerError, "Something went wrong..."));
        }

        private async Task<IResult<object>> IsValidAsync(SetAdminCommand command)
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

            var resultUserToSetExists = await _userExistsValidationRule.ValidateAsync(command.UserIdToSetAdmin);

            if (!resultUserToSetExists.IsSuccess)
            {
                return resultUserToSetExists;
            }

            var resultUserToSetIsConnectedToSmartHomeEntity = await _userIsConnectedToSmartHomeEntityValidationRule
               .ValidateAsync(new UserIsConnectedToSmartHomeEntityValidationRuleData
               {
                   SmartHomeEntityId = command.SmartHomeEntityId,
                   UserId = command.UserIdToSetAdmin
               });

            if (!resultUserToSetIsConnectedToSmartHomeEntity.IsSuccess)
            {
                return resultUserToSetIsConnectedToSmartHomeEntity;
            }
        
            return Result<object>.Success();
        }

        private async Task<bool> SetAdminForUser(SetAdminCommand command)
        {
            var entity = await _context.UserSmartHomeEntities.FirstOrDefaultAsync(x => x.UserId == command.UserIdToSetAdmin && x.SmartHomeEntityId == command.SmartHomeEntityId);
            entity.IsAdmin = command.IsAdmin;

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
