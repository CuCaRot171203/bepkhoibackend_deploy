using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class OrderCancellationHistory
    {
        public int OrderCancellationHistoryId { get; set; }
        public int OrderId { get; set; }
        public int CashierId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = null!;

        public virtual User Cashier { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
        public virtual Menu Product { get; set; } = null!;
    }
}
