using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.MenuDto
{
    public class MenuQrDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int ProductCategoryId { get; set; }
        public decimal SellPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? ProductVat { get; set; }
        public int UnitId { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? Status { get; set; }
        public List<string>? ProductImageUrls { get; set; }
    }
}
