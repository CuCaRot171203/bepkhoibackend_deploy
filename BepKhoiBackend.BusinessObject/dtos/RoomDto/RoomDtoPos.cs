using BepKhoiBackend.BusinessObject.dtos.OrderDto;

namespace BepKhoiBackend.BusinessObject.dtos.RoomDto
{
    public class RoomDtoPos
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int RoomAreaId { get; set; }
        public int? OrdinalNumber { get; set; }
        public int? SeatNumber { get; set; }
        public string? RoomNote { get; set; }
        public bool? IsUse { get; set; }
    }
}