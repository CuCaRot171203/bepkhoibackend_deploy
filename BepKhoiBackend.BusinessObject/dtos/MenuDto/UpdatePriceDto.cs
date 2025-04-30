using System.ComponentModel.DataAnnotations;

namespace BepKhoiBackend.BusinessObject.dtos.MenuDto
{
    public class UpdatePriceDto
    {
        [Required(ErrorMessage = "Product ID is required for updating price.")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Cost price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Cost price must be a non-negative number.")]
        public decimal CostPrice { get; set; }
        [Required(ErrorMessage = "Sell price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Sell price must be a non-negative number.")]
        public decimal SellPrice { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Sale price must be a non-negative number.")]
        public decimal? SalePrice { get; set; }
        [Range(0, 100, ErrorMessage = "Product VAT must be between 0 and 100.")]
        public decimal? ProductVat { get; set; }
    }
}
