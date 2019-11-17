using Microsoft.AspNetCore.Mvc;
using SmartHome.API.Infrastructure;
using SmartHome.API.Infrastructure.Extensions;
using SmartHome.BusinessLogic.Components.Queries;
using SmartHome.BusinessLogic.Components.QueryHandlers;
using System.Threading.Tasks;

namespace SmartHome.API.Components
{
    public class ComponentsController
    {
        private const string Route = "api/components";

        private readonly GetUnconnectedComponentsQueryHandler _getUnconnectedComponentsQueryHandler;

        public ComponentsController(GetUnconnectedComponentsQueryHandler getUnconnectedComponentsQueryHandler)
        {
            _getUnconnectedComponentsQueryHandler = getUnconnectedComponentsQueryHandler;
        }

        [HttpGet(Route + "/getUnconnected")]
        public async Task<IActionResult> GetUnconnectedComponents([FromQuery]UserIdSmartHomeEntityIdQueryParam queryParam)
        {
            var result = await _getUnconnectedComponentsQueryHandler.HandleAsync(new GetUnconnectedComponentsQuery
            {
                UserId  = queryParam.RequestedByUserId,
                SmartHomeEntityId = queryParam.SmartHomeEntityId
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(result.Data);
        }

    }
}