using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.OrderDto
{
    public class OrderCancellationHistoryDto
    {
        public int OrderCancellationHistoryId { get; set; }
        public int OrderId { get; set; }
        public int CashierId { get; set; }
        public string CashierName { get; set; } = null!;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public string Reason { get; set; } = null!;
    }
}
