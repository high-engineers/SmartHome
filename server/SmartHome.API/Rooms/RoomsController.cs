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
        private readonly GetRoomByIdQueryHandler _getRoomByIdQueryHandler;

        public RoomsController(GetRoomsQueryHandler getRoomsQueryHandler, GetRoomByIdQueryHandler getRoomByIdQueryHandler)
        {
            _getRoomsQueryHandler = getRoomsQueryHandler;
            _getRoomByIdQueryHandler = getRoomByIdQueryHandler;
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

        [HttpGet(Route + "/{roomId}")]
        public async Task<IActionResult> GetRoomById([FromQuery] Guid userId, [FromQuery] Guid smartHomeEntityId, [FromRoute] Guid roomId)
        {
            var result = await _getRoomByIdQueryHandler.HandleAsync(new GetRoomByIdQuery
            {
                SmartHomeEntityId = smartHomeEntityId,
                UserId = userId,
                RoomId = roomId
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(result);
        }
    }
}
