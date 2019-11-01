using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHome.BusinessLogic.Home.Queries;
using SmartHome.BusinessLogic.Infrastructure.Extensions;
using SmartHome.Data;
using System.Threading.Tasks;

namespace SmartHome.BusinessLogic.Home.QueryHandlers
{
    public class GetRoomByIdQueryHandler
    {
        private SmartHomeContext _dataContext;

        public GetRoomByIdQueryHandler(SmartHomeContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ObjectResult> HandleAsync(GetRoomByIdQuery query)
        {
            var room = await _dataContext.Rooms.SingleAsync(x => x.RoomId == query.Id);

            return room.ToSuccessfulResult();
        }
    }
}
