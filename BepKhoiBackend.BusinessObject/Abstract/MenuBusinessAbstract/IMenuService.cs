using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.Shared.Helpers;

namespace BepKhoiBackend.BusinessObject.Abstract.MenuBusinessAbstract
{
    public interface IMenuService
    {
        Task<ResultWithList<Menu>> GetAllMenusAsync(
            string sortBy, string sortDirection,
            int? categoryId, bool? isActive, string? productNameOrId);

        Task<ResultWithList<MenuCustomerDto>> GetAllMenusCustomerAsync(
            string sortBy, string sortDirection,
            int? categoryId, bool? isActive, string? productNameOrId);
        Task<PagedResult<Menu>> GetMenuByIdAsync(int pId);
        //Task<PagedResult<Menu>> AddMenuAsync(Menu menu);
        Task<PagedResult<MenuDto>> AddMenuAsync(CreateMenuDto menuDto, List<string> imageUrls);
        Task<Result<Menu>> UpdateMenuAsync(int productId, UpdateMenuDto dto, List<string> imageUrls);
        Task<PagedResult<Menu>> DeleteMenuAsync(int id);
        Task<(byte[] fileContent, string FileName, bool HasData, string ErrorMessage)> ExportActiveProductsToExcelAsync(
        string sortBy, string sortDirection, int? categoryId = null, bool? isActive = null);
        Task<(byte[] fileContent, string FileName, bool HasData, string ErrorMessage)> ExportPriceExcelAsync(
            string sortBy, string sortDirection, int? categoryId = null, bool? isActive = null);
        Task<(bool IsSuccess, string Message, Menu Data)> UpdatePriceOfProductAsync(UpdatePriceDto dto);
        Task<IEnumerable<MenuPosDto>> GetAllMenuPosAsync();
        Task<List<MenuPosDto>> FilterMenuAsyncPos(int? categoryId, bool? isAvailable);
        Task<IEnumerable<MenuQrDto>> GetAllMenuQrAsync();
    }
}
