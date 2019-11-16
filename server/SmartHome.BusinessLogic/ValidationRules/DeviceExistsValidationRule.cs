using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using SmartHome.Data.Models.Extenstions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class DeviceExistsValidationRule : IValidationRule<Guid>
    {
        private const string _deviceDoesntExistErrorMessage = "DeviceDoesntExistError";
        private readonly SmartHomeContext _context;

        public DeviceExistsValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(Guid componentId)
        {
            var component = await _context.Components.Where(x => x.ComponentId == componentId)
                .Include(x => x.ComponentType)
                .FirstOrDefaultAsync();

            if (component != null)
            {
                if (component.IsDevice())
                {
                    return Result<object>.Success();
                }
            }
            return Result<object>.Fail(new ResultError(StatusCodes.Status404NotFound, _deviceDoesntExistErrorMessage));
        }
    }
}
