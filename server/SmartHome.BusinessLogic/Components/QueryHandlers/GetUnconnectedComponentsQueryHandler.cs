using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Components.Models;
using SmartHome.BusinessLogic.Components.Queries;
using SmartHome.BusinessLogic.Infrastructure.Extensions;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using SmartHome.Data.Infrastructure.Enums;
using SmartHome.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Components.QueryHandlers
{
    public class GetUnconnectedComponentsQueryHandler
    {
        private SmartHomeContext _context;
        private readonly SmartHomeEntityExistsValidationRule _smartHomeEntityExistsValidationRule;
        private readonly UserExistsValidationRule _userExistsValidationRule;
        private readonly UserIsConnectedToSmartHomeEntityValidationRule _userIsConnectedToSmartHomeEntityValidationRule;
        private readonly UserIsAdminValidationRule _userCanSeeUnconnectedComponents;

        public GetUnconnectedComponentsQueryHandler(SmartHomeContext context, SmartHomeEntityExistsValidationRule smartHomeEntityExistsValidationRule, UserExistsValidationRule userExistsValidationRule, UserIsConnectedToSmartHomeEntityValidationRule userIsConnectedToSmartHomeEntityValidationRule, UserIsAdminValidationRule userIsAdminValidationRule)
        {
            _context = context;
            _smartHomeEntityExistsValidationRule = smartHomeEntityExistsValidationRule;
            _userExistsValidationRule = userExistsValidationRule;
            _userIsConnectedToSmartHomeEntityValidationRule = userIsConnectedToSmartHomeEntityValidationRule;
            _userCanSeeUnconnectedComponents = userIsAdminValidationRule;
        }

        public async Task<IResult<IReadOnlyCollection<ComponentBasicInfo>>> HandleAsync(GetUnconnectedComponentsQuery query)
        {
            var validationResult = await IsValidAsync(query);

            if (!validationResult.IsSuccess)
            {
                return Result<IReadOnlyCollection<ComponentBasicInfo>>.Fail(validationResult.ResultError);
            }

            var components = await GetUnconnectedComponentsAsync(query);
            return BuildBasicComponentsInfo(components).ToSuccessfulResult();
        }

        private async Task<IResult<object>> IsValidAsync(GetUnconnectedComponentsQuery query)
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

            var resultUserCanSeeUnconnectedComponents = await _userCanSeeUnconnectedComponents.ValidateAsync(new UserIsAdminValidationRuleData
            {
                UserId = query.UserId,
                SmartHomeEntityId = query.SmartHomeEntityId
            });

            if (!resultUserCanSeeUnconnectedComponents.IsSuccess)
            {
                return resultUserCanSeeUnconnectedComponents;
            }

            return Result<object>.Success();
        }

        private async Task<IReadOnlyCollection<Component>> GetUnconnectedComponentsAsync(GetUnconnectedComponentsQuery query)
        {
            return await
                _context
                .Components
                .Where(x => x.RoomId == null)
                .Include(x => x.ComponentType)
                .ToListAsync();
        }

        private IReadOnlyCollection<ComponentBasicInfo> BuildBasicComponentsInfo(IReadOnlyCollection<Component> components)
        {
            return components
                .Select(x => new ComponentBasicInfo
                {
                    Id = x.ComponentId,
                    Type = x.ComponentType.Type.ToString()
                })
                .ToList();
        }
    }
}
