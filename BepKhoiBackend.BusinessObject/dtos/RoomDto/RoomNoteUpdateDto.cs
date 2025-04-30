using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.RoomDto
{
    public class RoomNoteUpdateDto
    {
        public int RoomId { get; set; }
        public string RoomNote { get; set; } = string.Empty;
    }
}
