namespace BepKhoiBackend.BusinessObject.dtos.OrderDetailDto
{
    public class AddNoteRequest
    {
        public int OrderId { get; set; }
        public string? OrderNote { get; set; }
    }
}
