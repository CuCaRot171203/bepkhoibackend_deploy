using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.OrderDetailDto
{
    public class SplitOrderPosRequest_ProductList
    {
        public int Order_detail_id { get; set; }
        public int Product_id { get; set; }
        public int Quantity { get; set; }
    }
}
