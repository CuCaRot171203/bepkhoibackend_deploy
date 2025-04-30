using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.InvoiceDto
{
    public class InvoiceDetailForPaymentDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? ProductVat { get; set; }
        public string? ProductNote { get; set; }
    }
}
