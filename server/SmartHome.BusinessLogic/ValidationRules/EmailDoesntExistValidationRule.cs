using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class EmailDoesntExistValidationRule : IValidationRule<string>
    {
        private const string EmailExistsErrorMessage = "EmailExistsError";

        private readonly SmartHomeContext _context;

        public EmailDoesntExistValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(string email)
        {
            var result = await _context.Users.AnyAsync(x => x.Email == email);

            return result
                ? Result<object>.Fail(new ResultError(StatusCodes.Status409Conflict, EmailExistsErrorMessage))
                : Result<object>.Success();
        }
    }

}
