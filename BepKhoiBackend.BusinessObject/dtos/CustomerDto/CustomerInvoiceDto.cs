using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.CustomerDto
{
    public class CustomerInvoiceDto
    {
        public int InvoiceId { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public int CashierId { get; set; }
        public decimal AmountDue { get; set; }
    }
}
