using Microsoft.AspNetCore.Mvc;

namespace SmartHome.BusinessLogic.Infrastructure.Extensions
{
    internal static class ObjectResultExtensions
    {
        public static ObjectResult ToSuccessfulResult<T>(this T ourObject)
        {
            return new OkObjectResult(ourObject);
        }
    }
}
