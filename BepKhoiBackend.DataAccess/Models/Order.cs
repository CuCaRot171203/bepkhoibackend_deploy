using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class Order
    {
        public Order()
        {
            Invoices = new HashSet<Invoice>();
            OrderCancellationHistories = new HashSet<OrderCancellationHistory>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? ShipperId { get; set; }
        public int? DeliveryInformationId { get; set; }
        public int OrderTypeId { get; set; }
        public int? RoomId { get; set; }
        public DateTime CreatedTime { get; set; }
        public int TotalQuantity { get; set; }
        public decimal AmountDue { get; set; }
        public int OrderStatusId { get; set; }
        public string? OrderNote { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual DeliveryInformation? DeliveryInformation { get; set; }
        public virtual OrderStatus OrderStatus { get; set; } = null!;
        public virtual OrderType OrderType { get; set; } = null!;
        public virtual Room? Room { get; set; }
        public virtual User? Shipper { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<OrderCancellationHistory> OrderCancellationHistories { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
