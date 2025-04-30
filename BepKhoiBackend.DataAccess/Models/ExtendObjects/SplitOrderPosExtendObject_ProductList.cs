using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Models.ExtendObjects
{
    public class SplitOrderPosExtendObject_ProductList
    {
        public int Order_detail_id { get; set; }
        public int Product_id { get; set; }
        public int Quantity { get; set; }
    }
}
