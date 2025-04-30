using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class RoomArea
    {
        public RoomArea()
        {
            Rooms = new HashSet<Room>();
        }

        public int RoomAreaId { get; set; }
        public string RoomAreaName { get; set; } = null!;
        public string? RoomAreaNote { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
