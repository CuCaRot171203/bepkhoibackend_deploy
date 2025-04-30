using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class DeliveryInformation
    {
        public DeliveryInformation()
        {
            Orders = new HashSet<Order>();
        }

        public int DeliveryInformationId { get; set; }
        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public string ReceiverAddress { get; set; } = null!;
        public string? DeliveryNote { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
