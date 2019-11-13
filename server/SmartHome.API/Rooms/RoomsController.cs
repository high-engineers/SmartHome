using Microsoft.AspNetCore.Mvc;
using SmartHome.API.Infrastructure.Extensions;
using SmartHome.API.Rooms.Mappers;
using SmartHome.BusinessLogic.Rooms.Queries;
using SmartHome.BusinessLogic.Rooms.QueryHandlers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.API.Rooms
{
    public class RoomsController
    {
        public const string Route = "api/rooms";

        private readonly GetRoomsQueryHandler _getRoomsQueryHandler;

        public RoomsController(GetRoomsQueryHandler getRoomsQueryHandler)
        {
            _getRoomsQueryHandler = getRoomsQueryHandler;
        }

        [HttpGet(Route)]
        public async Task<IActionResult> GetRooms([FromQuery] Guid userId, [FromQuery] Guid smartHomeEntityId)
        {
            var result = await _getRoomsQueryHandler.HandleAsync(new GetRoomsQuery
            {
                SmartHomeEntityId = smartHomeEntityId,
                RequestedByUserId = userId
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(result.Data.Select(x => x.ToWebModel()));
        }
    }
}
