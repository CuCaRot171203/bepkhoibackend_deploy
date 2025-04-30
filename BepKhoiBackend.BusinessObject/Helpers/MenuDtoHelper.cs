using BepKhoiBackend.BusinessObject.dtos.MenuDto;

namespace BepKhoiBackend.BusinessObject.Helpers
{
    public static class MenuDtoHelper
    {
        public static (bool isValid, string errorMessage) ValidateUpdatePriceDto(UpdatePriceDto dto)
        {
            if (dto.CostPrice < 0 || dto.SellPrice < 0 || (dto.SalePrice.HasValue && dto.SalePrice < 0) || (dto.ProductVat.HasValue && dto.ProductVat < 0))
                return (false, "Cost price, sell price, sale price, and VAT must be greater than or equal to 0.");
            return (true, null);
        }
    }
}
