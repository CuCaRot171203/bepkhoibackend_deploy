namespace BepKhoiBackend.BusinessObject.dtos.OrderDetailDto
{
    public class UpdateOrderDetailQuantityRequest
    {
        public int OrderId { get; set; } 
        public int OrderDetailId { get; set; } 
        public bool? IsAdd { get; set; } 
        public int? Quantity { get; set; }
    }
}
