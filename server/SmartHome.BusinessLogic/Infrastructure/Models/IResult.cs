namespace SmartHome.BusinessLogic.Infrastructure.Models
{
    public interface IResult<T>
    {
        T Data { get; set; }
        bool IsSuccess { get; set; }
        ResultError ResultError { get; set; }
    }
}
