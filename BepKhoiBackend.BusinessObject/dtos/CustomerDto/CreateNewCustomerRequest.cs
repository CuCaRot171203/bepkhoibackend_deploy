using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.CustomerDto
{
    public class CreateNewCustomerRequest
    {
        public string Phone { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
    }
}
