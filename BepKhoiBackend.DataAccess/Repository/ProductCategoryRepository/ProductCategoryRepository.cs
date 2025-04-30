using BepKhoiBackend.DataAccess.Abstract.ProductCategoryAbstract;
using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BepKhoiBackend.DataAccess.Repository.ProductCategoryRepository
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly bepkhoiContext _context;

        public ProductCategoryRepository(bepkhoiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync()
        {
            return await _context.ProductCategories
                                 .Where(c => c.IsDelete == false || c.IsDelete == null)
                                 .ToListAsync();
        }
    }
}
