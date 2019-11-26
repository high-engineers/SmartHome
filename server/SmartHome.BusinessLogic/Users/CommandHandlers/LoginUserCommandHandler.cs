using SmartHome.BusinessLogic.Infrastructure.Extensions;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.Users.Commands;
using SmartHome.BusinessLogic.Users.Models;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Users.CommandHandlers
{
    public class LoginUserCommandHandler
    {
        private readonly SmartHomeContext _context;
        private readonly UserLoginCredentialsValidValidationRule _userLoginCredentialsValidValidationRule;

        public LoginUserCommandHandler(SmartHomeContext context, UserLoginCredentialsValidValidationRule userLoginCredentialsValidValidationRule)
        {
            _context = context;
            _userLoginCredentialsValidValidationRule = userLoginCredentialsValidValidationRule;
        }

        public async Task<IResult<UserLoginSuccessful>> HandleAsync(LoginUserCommand command)
        {
            var validationResult = await IsValidAsync(command);

            if (!validationResult.IsSuccess)
            {
                return Result<UserLoginSuccessful>.Fail(validationResult.ResultError);
            }

            var userId = GetUserIdByUsername(command.Username);
            return new UserLoginSuccessful
            {
                Id = userId
            }.ToSuccessfulResult();
        }

        private async Task<IResult<object>> IsValidAsync(LoginUserCommand command)
        {
            var resultLoginCredentialsValid = await _userLoginCredentialsValidValidationRule.ValidateAsync(new UserLoginCredentialsValidationRuleData
            {
                Username = command.Username,
                Password = command.Password
            });

            if (!resultLoginCredentialsValid.IsSuccess)
            {
                return resultLoginCredentialsValid;
            }

            return Result<object>.Success();
        }

        private Guid GetUserIdByUsername(string username)
        {
            return _context.Users.FirstOrDefault(x => x.Username == username).UserId;
        }
    }
}
