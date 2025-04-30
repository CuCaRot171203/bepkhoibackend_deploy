using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class OrderStatus
    {
        public OrderStatus()
        {
            Orders = new HashSet<Order>();
        }

        public int OrderStatusId { get; set; }
        public string OrderStatusTitle { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; }
    }
}
