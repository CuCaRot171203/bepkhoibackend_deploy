using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.InvoiceDto
{
    public class InvoiceForPaymentDto
    {
        public int PaymentMethodId { get; set; }
        public int OrderId { get; set; }
        public int OrderTypeId { get; set; }
        public int CashierId { get; set; }
        public int? ShipperId { get; set; }
        public int? CustomerId { get; set; }
        public int? RoomId { get; set; }
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
    }
}
