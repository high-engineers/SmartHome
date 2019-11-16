using Microsoft.AspNetCore.Mvc;
using SmartHome.API.Infrastructure;
using SmartHome.API.Infrastructure.Extensions;
using SmartHome.BusinessLogic.Rooms.Queries;
using SmartHome.BusinessLogic.Rooms.QueryHandlers;
using System;
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
        public async Task<IActionResult> GetRooms([FromQuery] UserIdSmartHomeEntityIdQueryParam queryParam)
        {
            var result = await _getRoomsQueryHandler.HandleAsync(new GetRoomsQuery
            {
                SmartHomeEntityId = queryParam.SmartHomeEntityId,
                RequestedByUserId = queryParam.UserId
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(result.Data);
        }


        [HttpGet(Route + "/{roomId}")]
        public async Task<IActionResult> GetRoomById([FromQuery] UserIdSmartHomeEntityIdQueryParam queryParam, [FromRoute] Guid roomId)
        {
            var result = await _getRoomByIdQueryHandler.HandleAsync(new GetRoomByIdQuery
            {
                SmartHomeEntityId = queryParam.SmartHomeEntityId,
                UserId = queryParam.UserId,
                RoomId = roomId
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(result.Data);
        }

    }
}
