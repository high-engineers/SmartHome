using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class UserExistsValidationRule : IValidationRule<Guid>
    {
        private const string UserDoesntExistErrorMessage = "UserDoesntExistError";

        private readonly SmartHomeContext _context;

        public UserExistsValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(Guid data)
        {
            var result = await _context.Users.AnyAsync(x => x.UserId == data);

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status404NotFound, UserDoesntExistErrorMessage));
        }
    }
}
