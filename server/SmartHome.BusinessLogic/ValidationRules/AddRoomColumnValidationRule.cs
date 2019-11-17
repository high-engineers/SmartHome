using Microsoft.AspNetCore.Http;
using SmartHome.BusinessLogic.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.ValidationRules
{
    public class AddRoomColumnValidationRule : IValidationRule<AddRoomColumnValidationRuleData>
    {
        private const string _roomNameIsRequiredErrorMessage = "RoomNameIsRequiredError";
        private const string _roomNameTooLongErrorMessage = "RoomNameTooLongError";
        private const string _roomTypeIsRequiredErrorMessage = "RoomTypeIsRequiredError";
        private const string _roomTypeTooLongErrorMessage = "RoomTypeTooLongError";

        public async Task<IResult<object>> ValidateAsync(AddRoomColumnValidationRuleData data)
        {
            var result = true;
            var errorMessages = new List<string>();

            if (data.Name == null)
            {
                errorMessages.Add(_roomNameIsRequiredErrorMessage);
                result = false;
            }
            else if (data.Name.Length > 30)
            {
                errorMessages.Add(_roomNameTooLongErrorMessage);
                result = false;
            }
            if (data.Type == null)
            {
                errorMessages.Add(_roomTypeIsRequiredErrorMessage);
                result = false;
            }
            else if (data.Type.Length > 30)
            {
                errorMessages.Add(_roomTypeTooLongErrorMessage);
                result = false;
            }

            return result
                ? Result<object>.Success()
                : Result<object>.Fail(new ResultError(StatusCodes.Status400BadRequest, string.Join(", ", errorMessages)));
        }
    }

    public class AddRoomColumnValidationRuleData
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
