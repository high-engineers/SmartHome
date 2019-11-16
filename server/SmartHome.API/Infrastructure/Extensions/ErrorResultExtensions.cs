using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.BusinessLogic.Infrastructure.Models;

namespace SmartHome.API.Infrastructure.Extensions
{
    public static class ErrorResultExtensions
    {
        public static IActionResult ToProperErrorResult(this ResultError resultError)
        {
            switch (resultError.StatusCode)
            {
                case StatusCodes.Status400BadRequest:
                    return new BadRequestObjectResult(resultError.ErrorMessage);
                case StatusCodes.Status401Unauthorized:
                    return new UnauthorizedObjectResult(resultError.ErrorMessage);
                case StatusCodes.Status403Forbidden:
                    return new ForbidResult();
                case StatusCodes.Status404NotFound:
                    return new NotFoundObjectResult(resultError.ErrorMessage);
                default:
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
