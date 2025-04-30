using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.UserDto.ShipperDto
{
    public class ShipperInvoiceDTO
    {
        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime CheckInTime { get; set; }
        public decimal AmountDue { get; set; }
        public bool? Status { get; set; }
        public int PaymentMethodId { get; set; } // ID phương thức thanh toán
        public string PaymentMethodName { get; set; } // Tên phương thức thanh toán
        public List<ShipperInvoiceDetailDTO> InvoiceDetails { get; set; }
    }

}
