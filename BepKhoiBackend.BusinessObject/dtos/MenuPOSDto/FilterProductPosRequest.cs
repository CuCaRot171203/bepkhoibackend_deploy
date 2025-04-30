using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.MenuPOSDto
{
    public class FilterProductPosRequest
    {
        public int? ProductCategoryId { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}
