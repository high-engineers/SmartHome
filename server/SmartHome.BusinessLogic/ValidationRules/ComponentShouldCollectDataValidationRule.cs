using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using SmartHome.Data.Infrastructure.Enums;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class ComponentShouldCollectDataValidationRule : IValidationRule<Guid>
    {
        private const string _componentShouldNotCollectDataErrorMessage = "ComponentShouldNotCollectDataError";
        private readonly SmartHomeContext _context;

        public ComponentShouldCollectDataValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(Guid componentId)
        {
            var component = await _context.Components.FirstOrDefaultAsync(x => x.ComponentId == componentId);

            var result = component.ComponentState != ComponentStateEnum.Registered;

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status400BadRequest, _componentShouldNotCollectDataErrorMessage));
        }
    }
}
