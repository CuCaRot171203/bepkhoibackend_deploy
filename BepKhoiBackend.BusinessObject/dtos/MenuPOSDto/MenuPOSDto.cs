using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.MenuPOSDto
{
    public class MenuPOSDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int ProductCategoryId { get; set; } // Thêm ProductCategoryId
        public string CategoryName { get; set; } = null!;
        public decimal SellPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? ProductVat { get; set; }
        public int UnitId { get; set; } // Thêm UnitId
        public string UnitTitle { get; set; } = null!;
        public bool? IsAvailable { get; set; }
        public bool? Status { get; set; }
    }
}
