using Microsoft.AspNetCore.Http;
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

        private readonly UsernameDoesntExistValidationRule _usernameDoesntExistRule;
        private readonly EmailDoesntExistValidationRule _emailDoesntExistRule;

        public RegisterUserCommandHandler(SmartHomeContext context, UsernameDoesntExistValidationRule usernameExistsValidationRule, EmailDoesntExistValidationRule emailExistsValidationRule)
        {
            _context = context;
            _usernameDoesntExistRule = usernameExistsValidationRule;
            _emailDoesntExistRule = emailExistsValidationRule;
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
                Password = command.Password
            };

            try
            {
                await _context.Users.AddAsync(newUser);
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
