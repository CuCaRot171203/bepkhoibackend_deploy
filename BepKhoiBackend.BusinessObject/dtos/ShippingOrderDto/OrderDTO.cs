using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.ShippingOrderDto
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? OrderNote { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; } = new();
    }
}
