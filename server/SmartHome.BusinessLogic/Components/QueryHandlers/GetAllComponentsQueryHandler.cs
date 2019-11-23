using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Components.Models;
using SmartHome.BusinessLogic.Components.Queries;
using SmartHome.BusinessLogic.Infrastructure.Extensions;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using SmartHome.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Components.QueryHandlers
{
    public class GetAllComponentsQueryHandler
    {
        private SmartHomeContext _context;
        private readonly SmartHomeEntityExistsValidationRule _smartHomeEntityExistsValidationRule;
        private readonly RoomExistsValidationRule _roomExistsValidationRule;

        public GetAllComponentsQueryHandler(SmartHomeContext context, SmartHomeEntityExistsValidationRule smartHomeEntityExistsValidationRule, RoomExistsValidationRule roomExistsValidationRule)
        {
            _context = context;
            _smartHomeEntityExistsValidationRule = smartHomeEntityExistsValidationRule;
            _roomExistsValidationRule = roomExistsValidationRule;
        }

        public async Task<IResult<IReadOnlyCollection<ComponentArduinoInfo>>> HandleAsync(GetAllComponentsQuery query)
        {
            var validationResult = await IsValidAsync(query);

            if (!validationResult.IsSuccess)
            {
                return Result<IReadOnlyCollection<ComponentArduinoInfo>>.Fail(validationResult.ResultError);
            }

            var components = await GetComponents(query);
            return BuildComponentArduinoInfoModel(components).ToSuccessfulResult();
        }

        private async Task<IResult<object>> IsValidAsync(GetAllComponentsQuery query)
        {
            var resultSmartHomeEntityExists = await _smartHomeEntityExistsValidationRule.ValidateAsync(query.SmartHomeEntityId);

            if (!resultSmartHomeEntityExists.IsSuccess)
            {
                return resultSmartHomeEntityExists;
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

        private async Task<IReadOnlyCollection<Component>> GetComponents(GetAllComponentsQuery query)
        {
            return await
                _context
                .Components
                .Where(x => x.RoomId == query.RoomId)
                .ToListAsync();
        }

        private IReadOnlyCollection<ComponentArduinoInfo> BuildComponentArduinoInfoModel(IReadOnlyCollection<Component> components)
        {
            return
                components
                .Select(x => new ComponentArduinoInfo
                {
                    Id = x.ComponentId,
                    State = x.ComponentState.ToString()
                })
                .ToList();
        }
    }

}
