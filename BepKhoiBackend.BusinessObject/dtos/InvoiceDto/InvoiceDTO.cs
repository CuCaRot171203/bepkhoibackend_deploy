using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.InvoiceDto
{
    public class InvoiceDTO
    {
        public int InvoiceId { get; set; }
        public string? PaymentMethod { get; set; }
        public int OrderId { get; set; }
        public string? OrderType { get; set; }
        public string? Cashier { get; set; }
        public string? Shipper { get; set; }
        public string? Customer { get; set; }
        public string? Room { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public int TotalQuantity { get; set; }
        public decimal Subtotal { get; set; }
        public decimal? OtherPayment { get; set; }
        public decimal? InvoiceDiscount { get; set; }
        public decimal? TotalVat { get; set; }
        public decimal AmountDue { get; set; }
        public bool? Status { get; set; }
        public string? InvoiceNote { get; set; }
        public List<InvoiceDetailDTO> InvoiceDetails { get; set; } = new List<InvoiceDetailDTO>();
    }
}
