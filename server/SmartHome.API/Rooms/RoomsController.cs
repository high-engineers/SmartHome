using Microsoft.AspNetCore.Mvc;
using SmartHome.API.Infrastructure;
using SmartHome.API.Infrastructure.Extensions;
using SmartHome.API.Infrastructure.PostModels;
using SmartHome.BusinessLogic.Components.CommandHandlers;
using SmartHome.BusinessLogic.Components.Commands;
using SmartHome.BusinessLogic.Rooms.CommandHandlers;
using SmartHome.BusinessLogic.Rooms.Commands;
using SmartHome.BusinessLogic.Rooms.Queries;
using SmartHome.BusinessLogic.Rooms.QueryHandlers;
using System;
using System.Threading.Tasks;

namespace SmartHome.API.Rooms
{
    public class RoomsController
    {
        private const string Route = "api/rooms";

        private readonly GetRoomsQueryHandler _getRoomsQueryHandler;
        private readonly GetRoomByIdQueryHandler _getRoomByIdQueryHandler;
        private readonly SwitchDeviceCommandHandler _switchDeviceCommandHandler;
        private readonly AddRoomCommandHandler _addRoomCommandHandler;

        public RoomsController(GetRoomsQueryHandler getRoomsQueryHandler, GetRoomByIdQueryHandler getRoomByIdQueryHandler, SwitchDeviceCommandHandler switchDeviceCommandHandler, AddRoomCommandHandler addRoomCommandHandler)
        {
            _getRoomsQueryHandler = getRoomsQueryHandler;
            _getRoomByIdQueryHandler = getRoomByIdQueryHandler;
            _switchDeviceCommandHandler = switchDeviceCommandHandler;
            _addRoomCommandHandler = addRoomCommandHandler;
        }

        [HttpGet(Route)]
        public async Task<IActionResult> GetRooms([FromQuery] UserIdSmartHomeEntityIdQueryParam queryParam)
        {
            var result = await _getRoomsQueryHandler.HandleAsync(new GetRoomsQuery
            {
                SmartHomeEntityId = queryParam.SmartHomeEntityId,
                UserId = queryParam.RequestedByUserId
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
                UserId = queryParam.RequestedByUserId,
                RoomId = roomId
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(result.Data);
        }

        [HttpPut(Route + "/{roomId}/components/{componentId}")]
        public async Task<IActionResult> SwitchDevice([FromQuery] UserIdSmartHomeEntityIdQueryParam queryParam, [FromRoute] Guid roomId, [FromRoute] Guid componentId, [FromQuery] bool newState)
        {
            var result = await _switchDeviceCommandHandler.HandleAsync(new SwitchDeviceCommand
            {
                UserId = queryParam.RequestedByUserId,
                SmartHomeEntityId = queryParam.SmartHomeEntityId,
                ComponentId = componentId,
                NewState = newState,
                RoomId = roomId
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(true);
        }

        [HttpPost(Route + "/add")]
        public async Task<IActionResult> AddRoom([FromBody] AddRoomModel newRoom, [FromQuery] UserIdSmartHomeEntityIdQueryParam queryParam)
        {
            var result = await _addRoomCommandHandler.HandleAsync(new AddRoomCommand
            {
                UserId = queryParam.RequestedByUserId,
                SmartHomeEntityId = queryParam.SmartHomeEntityId,
                Name = newRoom.Name,
                Type = newRoom.Type
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(true);
        }
    }
}
