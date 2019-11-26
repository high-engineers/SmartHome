﻿using SmartHome.BusinessLogic.Components.Models;
using SmartHome.BusinessLogic.Components.Queries;
using SmartHome.BusinessLogic.Infrastructure.Extensions;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Components.QueryHandlers
{
    public class GetRoomIdByComponentIdQueryHandler
    {
        private readonly SmartHomeContext _context;
        private readonly ComponentExistsValidationRule _componentExistsValidationRule;
        private readonly SmartHomeEntityExistsValidationRule _smartHomeEntityExistsValidationRule;

        public GetRoomIdByComponentIdQueryHandler(SmartHomeContext context, ComponentExistsValidationRule componentExistsValidationRule, SmartHomeEntityExistsValidationRule smartHomeEntityExistsValidationRule)
        {
            _context = context;
            _componentExistsValidationRule = componentExistsValidationRule;
            _smartHomeEntityExistsValidationRule = smartHomeEntityExistsValidationRule;
        }

        public async Task<IResult<ComponentRoomId>> HandleAsync(GetRoomIdByComponentIdQuery query)
        {
            var validationResult = await IsValidAsync(query);

            if (!validationResult.IsSuccess)
            {
                return Result<ComponentRoomId>.Fail(validationResult.ResultError);
            }

            var result = GetRoomIdByComponentId(query.ComponentId);
            return new ComponentRoomId
            {
                RoomId = result
            }.ToSuccessfulResult();
        }

        private async Task<IResult<object>> IsValidAsync(GetRoomIdByComponentIdQuery query)
        {
            var resultSmartHomeEntityExist = await _smartHomeEntityExistsValidationRule.ValidateAsync(query.SmartHomeEntityId);

            if (!resultSmartHomeEntityExist.IsSuccess)
            {
                return resultSmartHomeEntityExist;
            }

            var resultComponentExists = await _componentExistsValidationRule.ValidateAsync(query.ComponentId);

            if (!resultComponentExists.IsSuccess)
            {
                return resultComponentExists;
            }
            return Result<object>.Success();
        }

        private Guid GetRoomIdByComponentId(Guid componentId)
        {
            return _context.Components.FirstOrDefault(x => x.ComponentId == componentId).RoomId.Value;
        }
    }
}
