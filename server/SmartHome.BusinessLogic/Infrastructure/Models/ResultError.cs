namespace SmartHome.BusinessLogic.Infrastructure.Models
{
    public class ResultError
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }

        public ResultError(int statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}
