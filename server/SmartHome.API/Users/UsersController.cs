using Microsoft.AspNetCore.Mvc;
using SmartHome.API.Infrastructure.Extensions;
using SmartHome.API.Infrastructure.PostModels;
using SmartHome.BusinessLogic.Users.CommandHandlers;
using SmartHome.BusinessLogic.Users.Commands;
using System.Threading.Tasks;

namespace SmartHome.API.Users
{
    public class UsersController
    {
        private const string Route = "api/users";

        private readonly RegisterUserCommandHandler _registerUserCommandHandler;
        public UsersController(RegisterUserCommandHandler registerUserCommandHandler)
        {
            _registerUserCommandHandler = registerUserCommandHandler;
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
    }
}
