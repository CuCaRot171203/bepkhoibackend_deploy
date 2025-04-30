namespace BepKhoiBackend.BusinessObject.dtos.OrderDto
{
    public class CreateOrderRequestDto
    {
        public int? CustomerId { get; set; }
        public int? ShipperId { get; set; }
        public int? DeliveryInformationId { get; set; }
        public int OrderTypeId { get; set; }  
        public int? RoomId { get; set; }
        public int OrderStatusId { get; set; }  
        public string? OrderNote { get; set; }
    }
}
