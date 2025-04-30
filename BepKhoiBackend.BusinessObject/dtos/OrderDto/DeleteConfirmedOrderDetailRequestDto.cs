using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.OrderDto
{
    public class DeleteConfirmedOrderDetailRequestDto
    {
        public int OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public int CashierId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
