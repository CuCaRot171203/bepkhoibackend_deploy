namespace BepKhoiBackend.BusinessObject.dtos.OrderDetailDto
{
    public class OrderDetailDtoPos
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public bool? Status { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ProductNote { get; set; }
    }
}
