using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Infrastructure.Models
{
    internal interface IValidationRule<TData>
    {
        Task<IResult<object>> ValidateAsync(TData data);
    }
}
