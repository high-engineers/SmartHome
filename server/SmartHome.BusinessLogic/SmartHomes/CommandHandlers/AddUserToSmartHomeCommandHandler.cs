using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.SmartHomes.Commands;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using SmartHome.Data.Models;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.SmartHomes.CommandHandlers
{
    public class AddUserToSmartHomeCommandHandler
    {
        private readonly SmartHomeContext _context;
        private readonly SmartHomeEntityExistsValidationRule _smartHomeEntityExistsValidationRule;
        private readonly UserExistsValidationRule _userExistsValidationRule;
        private readonly UserIsConnectedToSmartHomeEntityValidationRule _userIsConnectedToSmartHomeEntityValidationRule;
        private readonly UserIsNotConnectedConnectedToSmartHomeEntityValidationRule _userIsNotConnectedConnectedToSmartHomeEntityValidationRule;
        private readonly UserIsAdminValidationRule _userIsAdminValidationRule;

        public AddUserToSmartHomeCommandHandler(SmartHomeContext context, SmartHomeEntityExistsValidationRule smartHomeEntityExistsValidationRule, UserExistsValidationRule userExistsValidationRule, UserIsConnectedToSmartHomeEntityValidationRule userIsConnectedToSmartHomeEntityValidationRule, UserIsNotConnectedConnectedToSmartHomeEntityValidationRule userIsNotConnectedConnectedToSmartHomeEntityValidationRule, UserIsAdminValidationRule userIsAdminValidationRule)
        {
            _context = context;
            _smartHomeEntityExistsValidationRule = smartHomeEntityExistsValidationRule;
            _userExistsValidationRule = userExistsValidationRule;
            _userIsConnectedToSmartHomeEntityValidationRule = userIsConnectedToSmartHomeEntityValidationRule;
            _userIsNotConnectedConnectedToSmartHomeEntityValidationRule = userIsNotConnectedConnectedToSmartHomeEntityValidationRule;
            _userIsAdminValidationRule = userIsAdminValidationRule;
        }

        public async Task<IResult<object>> HandleAsync(AddUserToSmartHomeCommand command)
        {
            var validationResult = await IsValidAsync(command);

            if (!validationResult.IsSuccess)
            {
                return Result<object>.Fail(validationResult.ResultError);
            }

            var result = await AddUserToSmartHomeAsync(command);
            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status500InternalServerError, "Something went wrong..."));
        }

        private async Task<IResult<object>> IsValidAsync(AddUserToSmartHomeCommand command)
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

            var resultUserToAssignExists = await _userExistsValidationRule.ValidateAsync(command.Email);

            if (!resultUserToAssignExists.IsSuccess)
            {
                return resultUserToAssignExists;
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

            var resultUserToAssignIsNotConnectedToSmartHomeEntity = await _userIsNotConnectedConnectedToSmartHomeEntityValidationRule.ValidateAsync(new UserIsNotConnectedConnectedToSmartHomeEntityValidationRuleData
            {
                Email = command.Email,
                SmartHomeEntityId = command.SmartHomeEntityId
            });

            if (!resultUserToAssignIsNotConnectedToSmartHomeEntity.IsSuccess)
            {
                return resultUserToAssignIsNotConnectedToSmartHomeEntity;
            }

            return Result<object>.Success();
        }

        private async Task<bool> AddUserToSmartHomeAsync(AddUserToSmartHomeCommand command)
        {
            var userToAssign = await _context.Users.FirstOrDefaultAsync(x => x.Email == command.Email);

            var newEntity = new UserSmartHomeEntity
            {
                SmartHomeEntityId = command.SmartHomeEntityId,
                UserId = userToAssign.UserId,
                IsAdmin = command.IsAdmin
            };

            try
            {
                await _context.UserSmartHomeEntities.AddAsync(newEntity);
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
