using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartHome.API.Infrastructure;
using SmartHome.API.Infrastructure.Extensions;
using SmartHome.API.Infrastructure.PostModels;
using SmartHome.API.Infrastructure.PutModels;
using SmartHome.BusinessLogic.SmartHomes.CommandHandlers;
using SmartHome.BusinessLogic.SmartHomes.Commands;

namespace SmartHome.API.SmartHomes
{
    public class SmartHomesController
    {
        private const string Route = "api/smarthomes";

        private readonly AddUserToSmartHomeCommandHandler _addUserToSmartHomeCommandHandler;
        private readonly ChangeSmartHomeNameCommandHandler _changeSmartHomeNameCommandHandler;

        public SmartHomesController(AddUserToSmartHomeCommandHandler addUserToSmartHomeCommandHandler, ChangeSmartHomeNameCommandHandler changeSmartHomeNameCommandHandler)
        {
            _addUserToSmartHomeCommandHandler = addUserToSmartHomeCommandHandler;
            _changeSmartHomeNameCommandHandler = changeSmartHomeNameCommandHandler;
        }

        [HttpPost(Route + "/assign")]
        public async Task<IActionResult> AddUserToSmartHome([FromQuery] UserIdSmartHomeEntityIdQueryParam queryParam, [FromBody] AssignUserToSmartHomeModel user)
        {
            var result = await _addUserToSmartHomeCommandHandler.HandleAsync(new AddUserToSmartHomeCommand
            {
                UserId = queryParam.RequestedByUserId,
                SmartHomeEntityId = queryParam.SmartHomeEntityId,
                Email = user.Email,
                IsAdmin = user.IsAdmin
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(true);
        }

        [HttpPut(Route + "/{smartHomeEntityId}")]
        public async Task<IActionResult> ChangeSmartHomeName([FromRoute] Guid smartHomeEntityId, [FromQuery] Guid requestedByUserId, [FromBody] ChangeSmartHomeNameModel name)
        {
            var result = await _changeSmartHomeNameCommandHandler.HandleAsync(new ChangeSmartHomeNameCommand
            {
                UserId = requestedByUserId,
                SmartHomeEntityId = smartHomeEntityId,
                NewName = name?.Name
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(true);
        }
        
    }
}