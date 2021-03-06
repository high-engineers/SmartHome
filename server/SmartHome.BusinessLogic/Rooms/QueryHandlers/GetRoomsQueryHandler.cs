﻿using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Components.Models;
using SmartHome.BusinessLogic.Infrastructure.Extensions;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.Rooms.Models;
using SmartHome.BusinessLogic.Rooms.Queries;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using SmartHome.Data.Infrastructure.Enums;
using SmartHome.Data.Models;
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

        public async Task<IResult<IReadOnlyCollection<RoomBasicInfo>>> HandleAsync(GetRoomsQuery query)
        {
            var validationResult = await IsValidAsync(query);

            if (!validationResult.IsSuccess)
            {
                return Result<IReadOnlyCollection<RoomBasicInfo>>.Fail(validationResult.ResultError);
            }

            var rooms = await GetRooms(query);

            return (rooms
                .Select(BuildRoom)
                .ToList() as IReadOnlyCollection<RoomBasicInfo>)
                .ToSuccessfulResult();
        }

        private async Task<IResult<object>> IsValidAsync(GetRoomsQuery query)
        {
            var resultSmartHomeEntityExists = await _smartHomeEntityExistsValidationRule.ValidateAsync(query.SmartHomeEntityId);

            if (!resultSmartHomeEntityExists.IsSuccess)
            {
                return resultSmartHomeEntityExists;
            }

            var resultUserExists = await _userExistsValidationRule.ValidateAsync(query.UserId);

            if (!resultUserExists.IsSuccess)
            {
                return resultUserExists;
            }

            var resultUserIsConnectedToSmartHomeEntity = await _userIsConnectedToSmartHomeEntityValidationRule
                .ValidateAsync(new UserIsConnectedToSmartHomeEntityValidationRuleData
                {
                    SmartHomeEntityId = query.SmartHomeEntityId,
                    UserId = query.UserId
                });

            if (!resultUserIsConnectedToSmartHomeEntity.IsSuccess)
            {
                return resultUserIsConnectedToSmartHomeEntity;
            }

            return Result<object>.Success();
        }

        private async Task<List<Room>> GetRooms(GetRoomsQuery query)
        {
            return await _context.Rooms
                .Where(x => x.SmartHomeEntityId == query.SmartHomeEntityId)
                .Include(x => x.Components)
                    .ThenInclude(x => x.ComponentType)
                .Include(x => x.Components)
                    .ThenInclude(x => x.ComponentData)
                .ToListAsync();
        }

        private RoomBasicInfo BuildRoom(Room room)
        {
            return new RoomBasicInfo
            {
                Id = room.RoomId,
                Name = room.Name,
                Type = room.Type,
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
