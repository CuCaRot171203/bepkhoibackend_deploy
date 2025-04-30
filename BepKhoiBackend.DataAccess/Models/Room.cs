using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class Room
    {
        public Room()
        {
            Invoices = new HashSet<Invoice>();
            Orders = new HashSet<Order>();
        }

        public int RoomId { get; set; }
        public string RoomName { get; set; } = null!;
        public int RoomAreaId { get; set; }
        public int? OrdinalNumber { get; set; }
        public int? SeatNumber { get; set; }
        public string? RoomNote { get; set; }
        public string? QrCodeUrl { get; set; }
        public bool? Status { get; set; }
        public bool? IsUse { get; set; }
        public bool? IsDelete { get; set; }

        public virtual RoomArea RoomArea { get; set; } = null!;
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
