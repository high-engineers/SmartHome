using Microsoft.AspNetCore.Http;
using SmartHome.BusinessLogic.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class RegisterUserColumnValidationRule : IValidationRule<RegisterUserColumnValidationRuleData>
    {
        private const string UsernameIsRequiredErrorMessage = "UsernameIsRequiredError";
        private const string UsernameIsTooLongErrorMessage = "UsernameIsTooLongError";
        private const string EmailIsRequiredErrorMessage = "EmailIsRequiredError";
        private const string EmailIsTooLongErrorMessage = "EmailIsTooLongError";
        private const string PasswordIsRequiredErrorMessage = "PasswordIsRequiredError";
        
        private const int UsernameColumnMaxLength = 20;
        private const int EmailColumnMaxLength = 40;

        public async Task<IResult<object>> ValidateAsync(RegisterUserColumnValidationRuleData data)
        {
            var result = true;
            var errorMessages = new List<string>();

            if (data.Username == null)
            {
                errorMessages.Add(UsernameIsRequiredErrorMessage);
                result = false;
            }
            else if (data.Username.Length > UsernameColumnMaxLength)
            {
                errorMessages.Add(UsernameIsTooLongErrorMessage);
                result = false;
            }
            if (data.Email == null)
            {
                errorMessages.Add(EmailIsRequiredErrorMessage);
                result = false;
            }
            else if (data.Email.Length > EmailColumnMaxLength)
            {
                errorMessages.Add(EmailIsTooLongErrorMessage);
                result = false;
            }
            if (data.Password == null)
            {
                errorMessages.Add(PasswordIsRequiredErrorMessage);
                result = false;
            }

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status400BadRequest, string.Join(", ", errorMessages)));
        }
    }

    public class RegisterUserColumnValidationRuleData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
