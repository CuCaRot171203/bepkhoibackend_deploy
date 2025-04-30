using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.TakeAwayOrderDto
{
    public class TakeAwayOrderDetailDTO
    {
        public int OrderDetailId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ProductNote { get; set; }
    }
}
