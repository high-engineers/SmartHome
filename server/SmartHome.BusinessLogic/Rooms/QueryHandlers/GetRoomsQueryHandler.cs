using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Components.Models;
using SmartHome.BusinessLogic.Infrastructure.Extensions;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.Rooms.Models;
using SmartHome.BusinessLogic.Rooms.Queries;
using SmartHome.BusinessLogic.Rooms.ValidationRules;
using SmartHome.Data;
using SmartHome.Data.Model.Enums;
using SmartHome.Data.Models.Extenstions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Rooms.QueryHandlers
{
    public class GetRoomsQueryHandler
    {
        private readonly SmartHomeContext _context;
        private readonly SmartHomeEntityExistsValidationRule _smartHomeEntityExistsValidationRule;
        private readonly UserExistsValidationRule _userExistsValidationRule;
        private readonly UserIsConnectedToSmartHomeEntityValidationRule _userIsConnectedToSmartHomeEntityValidationRule;

        public GetRoomsQueryHandler(SmartHomeContext context,
            SmartHomeEntityExistsValidationRule smartHomeEntityExistsValidationRule,
            UserExistsValidationRule userExistsValidationRule,
            UserIsConnectedToSmartHomeEntityValidationRule userIsConnectedToSmartHomeEntityValidationRule)
        {
            _context = context;
            _smartHomeEntityExistsValidationRule = smartHomeEntityExistsValidationRule;
            _userExistsValidationRule = userExistsValidationRule;
            _userIsConnectedToSmartHomeEntityValidationRule = userIsConnectedToSmartHomeEntityValidationRule;
        }

        public async Task<IResult<IReadOnlyCollection<Room>>> HandleAsync(GetRoomsQuery query)
        {
            var validationResult = await IsValidAsync(query);

            if (!validationResult.IsSuccess)
            {
                return Result<IReadOnlyCollection<Room>>.Fail(validationResult.ResultError);
            }

            var rooms = await GetRooms(query);

            return (rooms
                .Select(BuildRoom)
                .ToList() as IReadOnlyCollection<Room>)
                .ToSuccessfulResult();
        }

        private async Task<IResult<object>> IsValidAsync(GetRoomsQuery query)
        {
            var resultSmartHomeEntityExists = await _smartHomeEntityExistsValidationRule.ValidateAsync(query.SmartHomeEntityId);

            if (!resultSmartHomeEntityExists.IsSuccess)
            {
                return resultSmartHomeEntityExists;
            }

            var resultUserExists = await _userExistsValidationRule.ValidateAsync(query.RequestedByUserId);

            if (!resultUserExists.IsSuccess)
            {
                return resultUserExists;
            }

            var resultUserIsConnectedToSmartHomeEntity = await _userIsConnectedToSmartHomeEntityValidationRule
                .ValidateAsync(new UserIsConnectedToSmartHomeEntityValidationRuleData
                {
                    SmartHomeEntityId = query.SmartHomeEntityId,
                    UserId = query.RequestedByUserId
                });

            if (!resultUserIsConnectedToSmartHomeEntity.IsSuccess)
            {
                return resultUserIsConnectedToSmartHomeEntity;
            }

            return Result<object>.Success();
        }

        private Task<List<Data.Models.Room>> GetRooms(GetRoomsQuery query)
        {
            return _context.Rooms
                .Where(x => x.SmartHomeEntityId == query.SmartHomeEntityId)
                .Include(x => x.Components)
                    .ThenInclude(x => x.ComponentType)
                .Include(x => x.Components)
                    .ThenInclude(x => x.ComponentData)
                .ToListAsync();
        }

        private Room BuildRoom(Data.Models.Room room)
        {
            return new Room
            {
                Id = room.RoomId,
                Name = room.Name,
                Humidity = room.Components.GetCurrentHumidity(),
                Temperature = room.Components.GetCurrentTemperature(),
                Devices = room.Components
                    .Where(x => x.ComponentType.IsSwitchable)
                    .Select(device => new Device
                    {
                        Id = device.ComponentId,
                        IsOn = device.ComponentState == ComponentStateEnum.On,
                        Type = device.ComponentType.Type.ToString()
                    }).ToList()
            };
        }
    }
}
