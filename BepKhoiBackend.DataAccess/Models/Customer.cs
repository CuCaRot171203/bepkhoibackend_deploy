using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Invoices = new HashSet<Invoice>();
            Orders = new HashSet<Order>();
        }

        public int CustomerId { get; set; }
        public string Phone { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public bool? IsDelete { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
