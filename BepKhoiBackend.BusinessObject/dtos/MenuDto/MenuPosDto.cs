using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.MenuDto
{
    public class MenuPosDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int ProductCategoryId { get; set; }
        public decimal SellPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? ProductVat { get; set; }
        public int UnitId { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? Status { get; set; }
        public string? ProductImageUrl {  get; set; }
    }
}
