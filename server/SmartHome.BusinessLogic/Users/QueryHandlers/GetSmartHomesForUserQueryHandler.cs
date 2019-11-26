using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Infrastructure.Extensions;
using SmartHome.BusinessLogic.Infrastructure.Models;
using SmartHome.BusinessLogic.Users.Models;
using SmartHome.BusinessLogic.Users.Queries;
using SmartHome.BusinessLogic.ValidationRules;
using SmartHome.Data;
using SmartHome.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Users.QueryHandlers
{
    public class GetSmartHomesForUserQueryHandler
    {
        private readonly SmartHomeContext _context;
        private readonly UserExistsValidationRule _userExistsValidationRule;

        public GetSmartHomesForUserQueryHandler(SmartHomeContext context, UserExistsValidationRule userExistsValidationRule)
        {
            _context = context;
            _userExistsValidationRule = userExistsValidationRule;
        }

        public async Task<IResult<IReadOnlyCollection<SmartHomeBasicInfo>>> HandleAsync(GetSmartHomesForUserQuery query)
        {
            var validationResult = await IsValidAsync(query);

            if (!validationResult.IsSuccess)
            {
                return Result<IReadOnlyCollection<SmartHomeBasicInfo>>.Fail(validationResult.ResultError);
            }

            var smartHomes = await GetSmartHomeEntitiesForUser(query);
            return BuildSmartHomeBasicInfo(smartHomes).ToSuccessfulResult();
        }

        private async Task<IResult<object>> IsValidAsync(GetSmartHomesForUserQuery query)
        {
            var resultUserExists = await _userExistsValidationRule.ValidateAsync(query.UserId);

            if (!resultUserExists.IsSuccess)
            {
                return resultUserExists;
            }

            return Result<object>.Success();
        }

        private async Task<IReadOnlyCollection<SmartHomeEntity>> GetSmartHomeEntitiesForUser(GetSmartHomesForUserQuery query)
        {

            var ids = await
                _context
                .UserSmartHomeEntities
                .Where(x => x.UserId == query.UserId)
                .Select(x => x.SmartHomeEntityId)
                .ToListAsync();

            return await
                _context
                .SmartHomeEntities
                .Where(x => ids.Contains(x.SmartHomeEntityId))
                .ToListAsync();
        }

        private IReadOnlyCollection<SmartHomeBasicInfo> BuildSmartHomeBasicInfo(IReadOnlyCollection<SmartHomeEntity> smartHomes)
        {
            return
                smartHomes
                .Select(x => new SmartHomeBasicInfo
                {
                    Id = x.SmartHomeEntityId,
                    Name = x.Name
                })
                .ToList();
        }
    }
}
