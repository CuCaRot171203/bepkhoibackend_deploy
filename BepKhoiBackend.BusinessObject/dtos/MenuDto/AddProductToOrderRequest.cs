using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.MenuDto
{
    public class AddProductToOrderRequest
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
    }
}
