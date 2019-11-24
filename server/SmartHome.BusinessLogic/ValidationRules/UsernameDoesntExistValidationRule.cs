using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class UsernameDoesntExistValidationRule : IValidationRule<string>
    {
        private const string UsernameExistsErrorMessage = "UsernameExistsError";

        private readonly SmartHomeContext _context;

        public UsernameDoesntExistValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(string username)
        {
            var result = await _context.Users.AnyAsync(x => x.Username == username);

            return result
                ? Result<object>.Fail(new ResultError(StatusCodes.Status409Conflict, UsernameExistsErrorMessage))
                : Result<object>.Success();
        }
    }

}
