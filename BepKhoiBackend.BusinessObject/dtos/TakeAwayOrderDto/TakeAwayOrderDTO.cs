using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.TakeAwayOrderDto
{
    public class TakeAwayOrderDTO
    {
        public int OrderId { get; set; }
        public string? CustomerName { get; set; }
        public string OrderStatusName { get; set; } = string.Empty;
        public string OrderTypeName { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public int TotalQuantity { get; set; }
        public decimal AmountDue { get; set; }
        public string? OrderNote { get; set; }
        public List<TakeAwayOrderDetailDTO> OrderDetails { get; set; } = new();
    }
}
