using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BepKhoiBackend.BusinessObject.dtos.MenuDto
{
    public class CreateMenuDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name must not exceed 100 characters.")]
        public string ProductName { get; set; } = null!;
        [Required(ErrorMessage = "Product category is required.")]
        public int ProductCategoryId { get; set; }
        [Required(ErrorMessage = "Cost price required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Cost price must be a non-negative number.")]
        public decimal CostPrice { get; set; }
        [Required(ErrorMessage = "Sell price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Sell price must be a non-negative number.")]
        public decimal SellPrice { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Sale price must be a non-negative number.")]
        public decimal? SalePrice { get; set; }
        [Range(0, 100, ErrorMessage = "Product VAT must be between 0 and 100.")]
        public decimal? ProductVat { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Unit is required.")]
        public int UnitId { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? Status { get; set; }

        public IFormFile Image { get; set; }  // chỉ 1 ảnh

    }
}
