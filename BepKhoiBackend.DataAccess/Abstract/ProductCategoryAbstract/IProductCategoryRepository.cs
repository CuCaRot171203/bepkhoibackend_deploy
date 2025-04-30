using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.DataAccess.Abstract.ProductCategoryAbstract
{
    public interface IProductCategoryRepository
    {
        Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync();
    }
}
