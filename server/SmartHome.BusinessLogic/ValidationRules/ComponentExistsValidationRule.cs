using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class ComponentExistsValidationRule : IValidationRule<Guid>
    {
        private const string _componentDoesntExistErrorMessage = "ComponentDoesntExistError";

        private readonly SmartHomeContext _context;
        
        public ComponentExistsValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(Guid componentId)
        {
            var result = await _context
                .Components
                .AnyAsync(x => x.ComponentId == componentId);

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status400BadRequest, _componentDoesntExistErrorMessage));
        }
    }
}
