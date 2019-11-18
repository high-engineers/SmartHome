using Microsoft.AspNetCore.Http;
using SmartHome.BusinessLogic.Infrastructure.Models;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class AddDeviceToRoomColumnValidationRule : IValidationRule<string>
    {
        private const string _deviceNameIsRequiredErrorMessage = "DeviceNameIsRequiredError";
        private const string _deviceNameTooLongErrorMessage = "DeviceNameTooLongError";

        public async Task<IResult<object>> ValidateAsync(string name)
        {
            var result = true;
            string errorMessage = "";

            if (name == null)
            {
                errorMessage = _deviceNameIsRequiredErrorMessage;
                result = false;
            }
            else if (name.Length > 30)
            {
                errorMessage = _deviceNameTooLongErrorMessage;
                result = false;
            }

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status400BadRequest, errorMessage));
        }
    }
}
