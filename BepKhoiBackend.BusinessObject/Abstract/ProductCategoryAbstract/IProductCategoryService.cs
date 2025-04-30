using BepKhoiBackend.BusinessObject.dtos.ProductCategoryDto;

namespace BepKhoiBackend.BusinessObject.Abstract.ProductCategoryAbstract
{
    public interface IProductCategoryService
    {
        Task<IEnumerable<ProductCategoryDto>> GetProductCategoriesAsync();
    }
}
