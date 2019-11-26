using Microsoft.AspNetCore.Http;
using SmartHome.BusinessLogic.Infrastructure.Models;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class ChangeSmartHomeNameColumnValidationRule : IValidationRule<string>
    {
        private const string _nameIsRequiredErrorMessage = "NameIsRequiredError";
        private const string _nameIsTooLongErrorMessage = "NameIsTooLongError";

        public async Task<IResult<object>> ValidateAsync(string newName)
        {
            string errorMessage = "";
            bool result = true;

            if (newName == null)
            {
                errorMessage = _nameIsRequiredErrorMessage;
                result = false;
            }
            else if (newName.Length > 50)
            {
                errorMessage = _nameIsTooLongErrorMessage;
                result = false;
            }

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status400BadRequest, errorMessage));
        }
    }
}
