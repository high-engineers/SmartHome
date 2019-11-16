using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class SmartHomeEntityExistsValidationRule : IValidationRule<Guid>
    {
        private const string SmartHomeEntityDoesntExistErrorMessage = "SmartHomeEntityDoesntExistError";

        private readonly SmartHomeContext _context;

        public SmartHomeEntityExistsValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(Guid data)
        {
            var result = await _context.SmartHomeEntities.AnyAsync(x => x.SmartHomeEntityId == data);

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status404NotFound, SmartHomeEntityDoesntExistErrorMessage));
        }
    }
}
