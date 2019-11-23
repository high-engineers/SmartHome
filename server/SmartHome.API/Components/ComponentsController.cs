using Microsoft.AspNetCore.Mvc;
using SmartHome.API.Infrastructure;
using SmartHome.API.Infrastructure.Extensions;
using SmartHome.API.Infrastructure.PostModels;
using SmartHome.BusinessLogic.Components.CommandHandlers;
using SmartHome.BusinessLogic.Components.Commands;
using SmartHome.BusinessLogic.Components.Queries;
using SmartHome.BusinessLogic.Components.QueryHandlers;
using System;
using System.Threading.Tasks;

namespace SmartHome.API.Components
{
    public class ComponentsController
    {
        private const string Route = "api/components";

        private readonly GetUnconnectedComponentsQueryHandler _getUnconnectedComponentsQueryHandler;
        private readonly AddComponentToRoomCommandHandler _addComponentToRoomCommandHandler;
        private readonly GetAllComponentsQueryHandler _getAllComponentsQueryHandler;
        private readonly CollectSensorDataCommandHandler _collectSensorDataCommandHandler;

        public ComponentsController(GetUnconnectedComponentsQueryHandler getUnconnectedComponentsQueryHandler, AddComponentToRoomCommandHandler addComponentToRoomCommandHandler, GetAllComponentsQueryHandler getAllComponentsQueryHandler, CollectSensorDataCommandHandler collectSensorDataCommandHandler)
        {
            _getUnconnectedComponentsQueryHandler = getUnconnectedComponentsQueryHandler;
            _addComponentToRoomCommandHandler = addComponentToRoomCommandHandler;
            _getAllComponentsQueryHandler = getAllComponentsQueryHandler;
            _collectSensorDataCommandHandler = collectSensorDataCommandHandler;
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

        [HttpPut(Route + "/{componentId}")]
        public async Task<IActionResult> AddComponentToRoom([FromQuery] UserIdSmartHomeEntityIdQueryParam queryParam, [FromRoute] Guid componentId, [FromQuery] string name, [FromQuery] Guid roomId)
        {
            var result = await _addComponentToRoomCommandHandler.HandleAsync(new AddComponentToRoomCommand
            {
                UserId = queryParam.RequestedByUserId,
                SmartHomeEntityId = queryParam.SmartHomeEntityId,
                ComponentId = componentId,
                Name = name,
                RoomId = roomId
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(true);
        }

        [HttpGet(Route)]
        public async Task<IActionResult> GetAllComponents([FromQuery] SmartHomeEntityIdRoomIdQueryParam queryParam)
        {
            var result = await _getAllComponentsQueryHandler.HandleAsync(new GetAllComponentsQuery
            {
                SmartHomeEntityId = queryParam.SmartHomeEntityId,
                RoomId = queryParam.RoomId 
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(result.Data);
        }

        [HttpPost(Route + "/collect")]
        public async Task<IActionResult> CollectSensorData([FromQuery] SmartHomeEntityIdRoomIdQueryParam smartHomeRoomQueryParam, [FromBody] CollectSensorDataModel data)
        {
            var now = DateTime.Now;
            var result = await _collectSensorDataCommandHandler.HandleAsync(new CollectSensorDataCommand
            {
                ComponentId = data.SensorId,
                Reading = data.Reading,
                RoomId = smartHomeRoomQueryParam.RoomId,
                SmartHomeEntityId = smartHomeRoomQueryParam.SmartHomeEntityId,
                Timestamp = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0)
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(true);
        }
    }
}