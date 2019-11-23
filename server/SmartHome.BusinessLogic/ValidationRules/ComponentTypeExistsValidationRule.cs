using Microsoft.AspNetCore.Http;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.Data;
using SmartHome.Data.Infrastructure.Enums;
using System;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class ComponentTypeExistsValidationRule : IValidationRule<string>
    {
        private const string _componentTypeDoesntExistErrorMessage = "ComponentTypeDoesntExistError";
        private readonly SmartHomeContext _context;

        public ComponentTypeExistsValidationRule(SmartHomeContext context)
        {
            _context = context;
        }

        public async Task<IResult<object>> ValidateAsync(string type)
        {
            object outObject;
            var result = Enum.TryParse(typeof(ComponentTypeEnum), type, out outObject);
            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status400BadRequest, _componentTypeDoesntExistErrorMessage));
        }
    }
}
