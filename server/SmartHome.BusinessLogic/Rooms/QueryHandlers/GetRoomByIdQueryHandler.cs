//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SmartHome.BusinessLogic.Infrastructure.Extensions;
//using SmartHome.BusinessLogic.Rooms.Queries;
//using SmartHome.Data;
//using System.Threading.Tasks;

//namespace SmartHome.BusinessLogic.Rooms.QueryHandlers
//{
//    public class GetRoomByIdQueryHandler
//    {
//        private SmartHomeContext _context;

//        public GetRoomByIdQueryHandler(SmartHomeContext dataContext)
//        {
//            _context = dataContext;
//        }

//        public async Task<ObjectResult> HandleAsync(GetRoomByIdQuery query)
//        {
//            var room = await _context.Rooms.SingleAsync(x => x.RoomId == query.Id);

//            return room.ToSuccessfulResult();
//        }
//    }
//}
