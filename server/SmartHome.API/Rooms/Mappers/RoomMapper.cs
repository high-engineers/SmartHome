using SmartHome.API.Components.Mappers;
using SmartHome.API.Rooms.WebModels;
using SmartHome.BusinessLogic.Rooms.Models;
using System.Linq;

namespace SmartHome.API.Rooms.Mappers
{
    internal static class RoomMapper
    {
        public static RoomWebModel ToWebModel(this Room room)
        {
            return new RoomWebModel
            {
                Id = room.Id,
                Name = room.Name,
                Humidity = room.Humidity,
                Temperature = room.Temperature,
                Devices = room.Devices.Select(x => x.ToWebModel()).ToList()
            };
        }
    }
}
