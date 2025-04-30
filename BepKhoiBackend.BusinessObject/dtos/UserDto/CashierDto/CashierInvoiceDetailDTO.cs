using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.UserDto.CashierDto
{
    public class CashierInvoiceDetailDTO
    {
        public int InvoiceDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductVAT { get; set; }
        public string ProductNote { get; set; }
    }
}
