using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Infrastructure.Models
{
    public class Result<T> : IResult<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public ResultError ResultError { get; set; }

        public static Result<T> Success()
        {
            return new Result<T>
            {
                IsSuccess = true
            };
        }

        public static Result<T> Fail(ResultError resultError)
        {
            return new Result<T>
            {
                IsSuccess = false,
                ResultError = resultError
            };
        }

        public static Task<Result<T>> SuccessAsync()
        {
            return Task.FromResult(new Result<T>
            {
                IsSuccess = true
            });
        }

        public static Task<Result<T>> FailAsync(ResultError resultError)
        {
            return Task.FromResult(new Result<T>
            {
                IsSuccess = false,
                ResultError = resultError
            });
        }
    }
}
