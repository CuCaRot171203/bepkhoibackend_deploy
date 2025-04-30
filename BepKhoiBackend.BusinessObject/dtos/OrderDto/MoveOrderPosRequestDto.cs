using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.OrderDto
{
    public class MoveOrderPosRequestDto
    {
        public int OrderId { get; set; }
        public int OrderTypeId { get; set; }
        public int? RoomId { get; set; }
        public int? UserId { get; set; }
    }
}
