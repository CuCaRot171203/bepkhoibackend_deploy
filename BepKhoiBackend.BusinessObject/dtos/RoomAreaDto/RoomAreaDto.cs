using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.RoomAreaDto
{
    public class RoomAreaDto
    {
        public int RoomAreaId { get; set; }
        public string RoomAreaName { get; set; }
        public string RoomAreaNote { get; set; }
        public bool? IsDelete { get; set; }
    }
}
