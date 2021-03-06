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
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Rooms.QueryHandlers
{
    public class GetRoomByIdQueryHandler
    {
        private readonly SmartHomeContext _context;
        private readonly SmartHomeEntityExistsValidationRule _smartHomeEntityExistsValidationRule;
        private readonly UserExistsValidationRule _userExistsValidationRule;
        private readonly UserIsConnectedToSmartHomeEntityValidationRule _userIsConnectedToSmartHomeEntityValidationRule;
        private readonly RoomExistsValidationRule _roomExistsValidationRule;

        public GetRoomByIdQueryHandler(SmartHomeContext context, SmartHomeEntityExistsValidationRule smartHomeEntityExistsValidationRule, UserExistsValidationRule userExistsValidationRule, UserIsConnectedToSmartHomeEntityValidationRule userIsConnectedToSmartHomeEntityValidationRule, RoomExistsValidationRule roomExistsValidationRule)
        {
            _context = context;
            _smartHomeEntityExistsValidationRule = smartHomeEntityExistsValidationRule;
            _userExistsValidationRule = userExistsValidationRule;
            _userIsConnectedToSmartHomeEntityValidationRule = userIsConnectedToSmartHomeEntityValidationRule;
            _roomExistsValidationRule = roomExistsValidationRule;
        }

        public async Task<IResult<RoomDetails>> HandleAsync(GetRoomByIdQuery query)
        {
            var validationResult = await IsValidAsync(query);

            if (!validationResult.IsSuccess)
            {
                return Result<RoomDetails>.Fail(validationResult.ResultError);
            }

            var room = await GetRoomAsync(query);
            return BuildRoomDetails(room).ToSuccessfulResult();

        }

        private async Task<IResult<object>> IsValidAsync(GetRoomByIdQuery query)
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

            var resultRoomExists = await _roomExistsValidationRule.ValidateAsync(new RoomExistsValidationRuleData
            {
                RoomId = query.RoomId
            });

            if (!resultRoomExists.IsSuccess)
            {
                return resultRoomExists;
            }

            return Result<object>.Success();

        }

        private async Task<Room> GetRoomAsync(GetRoomByIdQuery query)
        {
            return await _context.Rooms
                .Where(x => x.SmartHomeEntityId == query.SmartHomeEntityId)
                .Include(x => x.Components)
                    .ThenInclude(x => x.ComponentType)
                .Include(x => x.Components)
                    .ThenInclude(x => x.ComponentData)
                .FirstOrDefaultAsync(x => x.RoomId == query.RoomId);
        }

        private RoomDetails BuildRoomDetails(Room room)
        {
            return new RoomDetails
            {
                Id = room.RoomId,
                Name = room.Name,
                Devices = room.Components
                    .Where(x => x.RoomId == room.RoomId && x.ComponentType.IsSwitchable)
                    .Select(x => new DeviceDetails
                    {
                        Id = x.ComponentId,
                        IsOn = x.ComponentState == ComponentStateEnum.On,
                        Type = x.ComponentType.Type.ToString(),
                        Name = x.Name
                    })
                    .ToList(),
                SensorsHistory = room.Components
                    .Where(x => x.RoomId == room.RoomId && !x.ComponentType.IsSwitchable)
                    .Select(x => new SensorHistory
                    {
                        Type = x.ComponentType.Type.ToString(),
                        HistoryEntries = x.ComponentData
                            .Where(q => q.Timestamp.Date == DateTime.Now.Date)
                            .Select(y => new SensorHistoryEntry
                            {
                                Id = y.ComponentDataId,
                                Time = y.Timestamp,
                                Value = y.Reading
                            }).ToList()
                    }).ToList()
            };
        }
    }
}
