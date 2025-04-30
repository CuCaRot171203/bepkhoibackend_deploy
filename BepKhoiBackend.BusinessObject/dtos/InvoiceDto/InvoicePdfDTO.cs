using BepKhoiBackend.BusinessObject.dtos.InvoiceDto;
namespace BepKhoiBackend.BusinessObject.dtos.InvoiceDto {

    public class InvoicePdfDTO
    {
        public int InvoiceId { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal Subtotal { get; set; } // Giá ban đầu
        public decimal TotalVat { get; set; } // VAT
        public decimal AmountDue { get; set; } // Tổng tiền
        public List<InvoiceDetailPdfDTO> InvoiceDetails { get; set; } = new List<InvoiceDetailPdfDTO>();
    }
}