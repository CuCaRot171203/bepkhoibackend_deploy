using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.CustomerDto
{
    public class AddCustomerToOrderRequest
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
    }
}
