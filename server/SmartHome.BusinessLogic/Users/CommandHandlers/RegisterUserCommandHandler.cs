using Microsoft.AspNetCore.Http;
using SmartHome.BusinessLogic.Infrastructure.Extensions;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.Users.Commands;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using SmartHome.Data.Models;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Users.CommandHandlers
{
    public class RegisterUserCommandHandler
    {
        private readonly SmartHomeContext _context;

        private readonly RegisterUserColumnValidationRule _registerUserColumnValidationRule;
        private readonly UsernameDoesntExistValidationRule _usernameDoesntExistRule;
        private readonly EmailDoesntExistValidationRule _emailDoesntExistRule;

        public RegisterUserCommandHandler(SmartHomeContext context, RegisterUserColumnValidationRule registerUserColumnValidationRule, UsernameDoesntExistValidationRule usernameDoesntExistRule, EmailDoesntExistValidationRule emailDoesntExistRule)
        {
            _context = context;
            _registerUserColumnValidationRule = registerUserColumnValidationRule;
            _usernameDoesntExistRule = usernameDoesntExistRule;
            _emailDoesntExistRule = emailDoesntExistRule;
        }

        public async Task<IResult<object>> HandleAsync(RegisterUserCommand command)
        {
            var validationResult = await IsValidAsync(command);

            if (!validationResult.IsSuccess)
            {
                return Result<object>.Fail(validationResult.ResultError);
            }
            
            var result = await AddUserAsync(command);
            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status500InternalServerError, "Something went wrong..."));
        }

        private async Task<IResult<object>> IsValidAsync(RegisterUserCommand command)
        {
            var resultColumnValidation = await _registerUserColumnValidationRule.ValidateAsync(new RegisterUserColumnValidationRuleData
            {
                Username = command.Username,
                Email = command.Email,
                Password = command.Password
            });

            if (!resultColumnValidation.IsSuccess)
            {
                return resultColumnValidation;
            }

            var resultUsernameDoesntExist = await _usernameDoesntExistRule.ValidateAsync(command.Username);

            if (!resultUsernameDoesntExist.IsSuccess)
            {
                return resultUsernameDoesntExist;
            }

            var resultEmailDoesntExist = await _emailDoesntExistRule.ValidateAsync(command.Email);

            if (!resultEmailDoesntExist.IsSuccess)
            {
                return resultEmailDoesntExist;
            }

            return Result<object>.Success();
        }

        private async Task<bool> AddUserAsync(RegisterUserCommand command)
        {
            var newUser = new User
            {
                Username = command.Username,
                Email = command.Email,
                Password = command.Password.GetHashedString()
            };

            try
            {
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
