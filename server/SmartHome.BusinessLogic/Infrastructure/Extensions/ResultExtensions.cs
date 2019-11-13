using SmartHome.BusinessLogic.Infrastructure.Models;

namespace SmartHome.BusinessLogic.Infrastructure.Extensions
{
    internal static class ResultExtensions
    {
        public static Result<T> ToSuccessfulResult<T>(this T ourObject)
        {
            return new Result<T>
            {
                Data = ourObject,
                IsSuccess = true
            };
        }
    }
}
