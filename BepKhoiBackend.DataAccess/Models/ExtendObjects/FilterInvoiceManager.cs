using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Models.ExtendObjects
{
    public class FilterInvoiceManager
    {
        public int? InvoiceId { get; set; }
        public string? CustomerKeyword { get; set; }
        public string? CashierKeyword { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? Status { get; set; }
        public int? PaymentMethod { get; set; }
    }
}
