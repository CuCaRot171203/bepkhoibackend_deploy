namespace BepKhoiBackend.BusinessObject.dtos.RoomDto
{
    public class RoomCreateDto
    {
        public string RoomName { get; set; }
        public int RoomAreaId { get; set; }
        public int? OrdinalNumber { get; set; }
        public int? SeatNumber { get; set; }
        public string? RoomNote { get; set; }
        
    }
}
