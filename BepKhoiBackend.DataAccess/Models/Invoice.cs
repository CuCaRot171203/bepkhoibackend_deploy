using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class Invoice
    {
        public Invoice()
        {
            InvoiceDetails = new HashSet<InvoiceDetail>();
        }

        public int InvoiceId { get; set; }
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

        public virtual User Cashier { get; set; } = null!;
        public virtual Customer? Customer { get; set; }
        public virtual Order Order { get; set; } = null!;
        public virtual OrderType OrderType { get; set; } = null!;
        public virtual PaymentMethod PaymentMethod { get; set; } = null!;
        public virtual Room? Room { get; set; }
        public virtual User? Shipper { get; set; }
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
