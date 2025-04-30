using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Models.ExtendObjects
{
    public class FilterOrderManager
    {
        public int? OrderId { get; set; }
        public string? CustomerKeyword { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? OrderStatus { get; set; }
        public int? Ordertype { get; set; }
    }
}
