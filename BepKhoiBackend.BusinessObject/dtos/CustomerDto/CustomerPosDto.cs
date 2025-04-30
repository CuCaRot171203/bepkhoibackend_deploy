using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.CustomerDto
{
    public class CustomerPosDto
    {
        public int customerId { get; set; }
        public string phone { get; set; } 
        public string customerName { get; set; } 
    }
}
