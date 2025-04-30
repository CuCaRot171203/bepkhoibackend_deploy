using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.OrderDto.PaymentDto
{
    public class CustomerPaymertDto
    {
        public int CustomerId { get; set; }
        public string Phone { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
    }
}
