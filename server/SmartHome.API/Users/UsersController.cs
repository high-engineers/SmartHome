using Microsoft.AspNetCore.Mvc;
using SmartHome.API.Infrastructure;
using SmartHome.API.Infrastructure.Extensions;
using SmartHome.API.Infrastructure.PostModels;
using SmartHome.API.Infrastructure.PutModels;
using SmartHome.BusinessLogic.Users.CommandHandlers;
using SmartHome.BusinessLogic.Users.Commands;
using SmartHome.BusinessLogic.Users.Queries;
using SmartHome.BusinessLogic.Users.QueryHandlers;
using System;
using System.Threading.Tasks;

namespace SmartHome.API.Users
{
    public class UsersController
    {
        private const string Route = "api/users";

        private readonly RegisterUserCommandHandler _registerUserCommandHandler;
        private readonly LoginUserCommandHandler _loginUserCommandHandler;
        private readonly SetAdminCommandHandler _setAdminCommandHandler;
        private readonly GetSmartHomesForUserQueryHandler _getSmartHomesForUserQueryHandler;

        public UsersController(RegisterUserCommandHandler registerUserCommandHandler, LoginUserCommandHandler loginUserCommandHandler, SetAdminCommandHandler setAdminCommandHandler, GetSmartHomesForUserQueryHandler getSmartHomesForUserQueryHandler)
        {
            _registerUserCommandHandler = registerUserCommandHandler;
            _loginUserCommandHandler = loginUserCommandHandler;
            _setAdminCommandHandler = setAdminCommandHandler;
            _getSmartHomesForUserQueryHandler = getSmartHomesForUserQueryHandler;
        }

        [HttpPost(Route + "/register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterCredentialsModel credentials)
        {
            var result = await _registerUserCommandHandler.HandleAsync(new RegisterUserCommand
            {
                Username = credentials.Username,
                Email = credentials.Email,
                Password = credentials.Password
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(true);
        }

        [HttpPost(Route + "/login")]
        public async Task<IActionResult> Login([FromBody] UserLoginCredentialsModel loginCredentials)
        {
            var result = await _loginUserCommandHandler.HandleAsync(new LoginUserCommand
            {
                Username = loginCredentials.Username,
                Password = loginCredentials.Password
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(result.Data);
        }

        [HttpPut(Route + "/setAdmin")]
        public async Task<IActionResult> SetAdmin([FromQuery] UserIdSmartHomeEntityIdQueryParam queryParam, [FromBody] SetAdminModel user)
        {
            var result = await _setAdminCommandHandler.HandleAsync(new SetAdminCommand
            {
                UserId = queryParam.RequestedByUserId,
                SmartHomeEntityId = queryParam.SmartHomeEntityId,
                IsAdmin = user.IsAdmin,
                UserIdToSetAdmin = user.UserId
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(true);
        }

        [HttpGet(Route + "/smartHomes")]
        public async Task<IActionResult> GetSmartHomesForUser([FromQuery] Guid requestedByUserId)
        {
            var result = await _getSmartHomesForUserQueryHandler.HandleAsync(new GetSmartHomesForUserQuery
            {
                UserId = requestedByUserId
            });

            if (!result.IsSuccess)
            {
                return result.ResultError.ToProperErrorResult();
            }

            return new OkObjectResult(result.Data);
        }

    }
}
