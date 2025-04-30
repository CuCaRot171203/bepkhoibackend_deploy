using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.OrderDetailDto
{
    public class SplitOrderPosRquest
    {
        public bool CreateNewOrder { get; set; }
        public int OrderId { get; set; }
        public int? SplitTo { get; set; } 
        public int OrderTypeId { get; set; }
        public int? RoomId { get; set; } 
        public int? ShipperId { get; set; } 
        public List<SplitOrderPosRequest_ProductList>? Product { get; set; }
            = new List<SplitOrderPosRequest_ProductList>();
    }
}
