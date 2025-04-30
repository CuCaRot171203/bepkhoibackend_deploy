using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class PaymentMethod
    {
        public PaymentMethod()
        {
            Invoices = new HashSet<Invoice>();
        }

        public int PaymentMethodId { get; set; }
        public string PaymentMethodTitle { get; set; } = null!;
        public bool? Status { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
