using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.OrderDto
{
    public class OrderGeneralDataPosDto
    {
        public int OrderId { get; set; }
        public string? OrderNote { get; set; }
        public int TotalQuantity { get; set; }
        public decimal AmountDue { get; set; }
        public bool HasUnconfirmProducts {  get; set; }
    }
}
