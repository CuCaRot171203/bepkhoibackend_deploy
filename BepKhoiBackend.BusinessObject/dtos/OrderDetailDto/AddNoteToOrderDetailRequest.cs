using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.OrderDetailDto
{
    public class AddNoteToOrderDetailRequest
    {
        public int OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
