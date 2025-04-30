using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class User
    {
        public User()
        {
            InvoiceCashiers = new HashSet<Invoice>();
            InvoiceShippers = new HashSet<Invoice>();
            OrderCancellationHistories = new HashSet<OrderCancellationHistory>();
            Orders = new HashSet<Order>();
        }

        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public bool? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsVerify { get; set; }
        public int UserInformationId { get; set; }
        public bool? IsDelete { get; set; }

        public virtual UserRole Role { get; set; } = null!;
        public virtual UserInformation UserInformation { get; set; } = null!;
        public virtual ICollection<Invoice> InvoiceCashiers { get; set; }
        public virtual ICollection<Invoice> InvoiceShippers { get; set; }
        public virtual ICollection<OrderCancellationHistory> OrderCancellationHistories { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
