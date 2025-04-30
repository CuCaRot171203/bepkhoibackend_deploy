using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class OrderType
    {
        public OrderType()
        {
            Invoices = new HashSet<Invoice>();
            Orders = new HashSet<Order>();
        }

        public int OrderTypeId { get; set; }
        public string OrderTypeTitle { get; set; } = null!;

        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
