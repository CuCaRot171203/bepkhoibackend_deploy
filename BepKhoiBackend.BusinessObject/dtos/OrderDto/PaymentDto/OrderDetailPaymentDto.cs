using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.OrderDto.PaymentDto
{
    public class OrderDetailPaymentDto
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public bool? Status { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ProductNote { get; set; }
        public decimal ProductVat { get; set; }
    }
}
