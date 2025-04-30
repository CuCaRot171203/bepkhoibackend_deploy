namespace BepKhoiBackend.BusinessObject.dtos.OrderDetailDto
{
    public class CancelOrderDetailResponse
    {
        public int OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public int CashierId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime CanceledAt { get; set; } = DateTime.UtcNow;
    }
}
