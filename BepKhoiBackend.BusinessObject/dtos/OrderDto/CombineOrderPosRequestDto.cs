using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.OrderDto
{
    public class CombineOrderPosRequestDto
    {
        public int? FirstOrderId { get; set; }
        public int? SecondOrderId { get; set; }
    }
}
