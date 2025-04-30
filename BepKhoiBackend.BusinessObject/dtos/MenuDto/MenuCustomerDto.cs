using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.MenuDto
{
    public class MenuCustomerDto
    {
        public MenuCustomerDto()
        {
            ProductImages = new HashSet<ProductImageDto>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int ProductCategoryId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? ProductVat { get; set; }
        public string? Description { get; set; }
        public int UnitId { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? Status { get; set; }
        public bool? IsDelete { get; set; }

        public virtual ICollection<ProductImageDto> ProductImages { get; set; }
    }


}
