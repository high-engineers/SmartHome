using System.Collections.Generic;

namespace SmartHome.API.Rooms.WebModels
{
    public class GetRoomsWebModel
    {
        public IReadOnlyCollection<RoomWebModel> Rooms { get; set; }
    }
}
