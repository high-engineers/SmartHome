using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class UserLoginCredentialsValidValidationRule : IValidationRule<UserLoginCredentialsValidationRuleData>
    {
        private const string _loginFailedErrorMessage = "LoginFailedError";

        private readonly SmartHomeContext _context;

        public UserLoginCredentialsValidValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(UserLoginCredentialsValidationRuleData credentials)
        {
            var result = await _context.Users.AnyAsync(x => x.Username == credentials.Username && x.Password == credentials.Password);

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status401Unauthorized, _loginFailedErrorMessage));
        }
    }

    public class UserLoginCredentialsValidationRuleData
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
