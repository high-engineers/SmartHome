using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.BusinessLogic.Home.Queries;
using SmartHome.BusinessLogic.Home.QueryHandlers;
using System;
using System.Threading.Tasks;

namespace SmartHome.API.Rooms
{
    public class RoomsController
    {
        private const string Route = "api/rooms/";

        private GetRoomByIdQueryHandler _getRoomByIdQueryHandler;

        public RoomsController(GetRoomByIdQueryHandler getRoomByIdQueryHandler)
        {
            _getRoomByIdQueryHandler = getRoomByIdQueryHandler;
        }

        [HttpGet(Route + "{id}")]
        public async Task<ActionResult> GetRoomById([FromRoute] Guid id)
        {
            var result = await _getRoomByIdQueryHandler.HandleAsync(new GetRoomByIdQuery
            {
                Id = id
            });

            if (result.StatusCode != StatusCodes.Status200OK)
            {
                return new BadRequestResult();
            }

            return new OkObjectResult(result.Value);
        }
    }
}
