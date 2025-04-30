using BepKhoiBackend.DataAccess.Abstract.MenuAbstract;
using BepKhoiBackend.DataAccess.Helpers;
using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using BepKhoiBackend.DataAccess.Repository.Base;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

public class MenuRepository : RepositoryBase, IMenuRepository
{
    private readonly bepkhoiContext _context;

    public MenuRepository(bepkhoiContext context, ILogger<MenuRepository> logger) : base(logger)
    {
        _context = context;
    }

    /*=========== SHARED FUNCTION ================*/

    // Method to check product have exist or not by Id
    public async Task<bool> CheckMenuExistById(int pId)
    {
        return await _context.Menus.AnyAsync(
            m => m.ProductId == pId);
    }

    // Method to check if product have to soft delete or not by id
    public async Task<bool> CheckMenuIsDelete(int pId)
    {
        var menu = await _context.Menus.FindAsync(pId);
        return (menu != null
            && menu.IsDelete == true);
    }

    // Method to check if product have exist name
    public async Task<bool> CheckMenuExistByName(string name)
    {
       return await _context.Menus.AllAsync(m => m.ProductName == name);
    }


    /*========= CALL METHOD ==========*/

    // Get all menu
    public IQueryable<Menu> GetMenusQueryable()
    {
        return _context.Menus
            .AsNoTracking()
            .Where(m => m.IsDelete != true);
    }

    // Get menu by id
    public async Task<Menu?> GetMenuByIdAsync(int pId)
    {
        if (pId <= 0)
        {
            throw new ArgumentException("Product ID must be greater than 0.", nameof(pId));
        }

        return await _context.Menus
            .Include(m=>m.ProductImages)
            .Where(m => m.ProductId == pId && m.IsDelete == false)
            .FirstOrDefaultAsync();
    }

    public async Task<Menu?> GetMenuByIdForUpdatePriceAsync(int pId)
    {
        if (pId <= 0)
        {
            throw new ArgumentException("Product ID must be greater than 0.", nameof(pId));
        }

        return await _context.Menus
            .Where(m => m.ProductId == pId && m.IsDelete == false)
            .FirstOrDefaultAsync();
    }

    // Add menu
    public async Task<Menu> AddMenuAsync(Menu menu)
    {
        return await ExecuteDbActionAsync(async () =>
        {
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            return menu;
        });
    }

    // Update Menu
    public async Task<Menu> UpdateMenuAsync(Menu menu)
    {
        _context.Menus.Update(menu);
        await _context.SaveChangesAsync();
        return menu;
    }

    // Soft delete (IsDelete = true)
    public async Task DeleteMenuAsync(Menu menu)
    {
        menu.IsDelete = true;
        _context.Menus.Update(menu);
        await _context.SaveChangesAsync();
    }

    // Get menu list by Id (Filter category, sort)
    public async Task<List<Menu>> GetMenusByConditionAsync(int? categoryId, string sortBy, string sortDirection)
    {
        var query = _context.Menus.Where(m => m.IsDelete == false).AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(m => m.ProductCategoryId == categoryId.Value);
        }

        query = MenuHelper.ApplySorting(query, sortBy, sortDirection);

        return await query.ToListAsync();
    }

    // Get filtered
    public async Task<IQueryable<Menu>> GetFilteredMenusAsync(int? categoryId)
    {
        var query = _context.Menus.Where(m => m.IsDelete == false).AsQueryable();

        if (categoryId.HasValue)
        {
            bool categoryExists = await _context.ProductCategories
                .AnyAsync(c => c.ProductCategoryId == categoryId.Value);
            if (!categoryExists) throw new ArgumentException($"Category ID {categoryId.Value} does not exist.");
            query = query.Where(m => m.ProductCategoryId == categoryId.Value);
        }

        return query;
    }

    // Update price of product
    public async Task UpdateMenuPriceAsync(Menu menu)
    {
        _context.Menus.Update(menu);
        await _context.SaveChangesAsync();
    }

    //Pham Son Tung
    //Func for api GetAllMenuPos 
    public async Task<IEnumerable<Menu>> GetAllMenuPos()
    {
        try
        {
            var menuList = await _context.Menus
                .AsNoTracking()
                .Include(m => m.ProductImages)
                .Where(m => m.IsDelete != true)
                .Where(m => m.Status == true)
                .OrderBy(m => m.ProductName)
                .Select(m => new Menu
                {
                    ProductId = m.ProductId,
                    ProductName = m.ProductName,
                    ProductCategoryId = m.ProductCategoryId,
                    SellPrice = m.SellPrice,
                    SalePrice = m.SalePrice,
                    ProductVat = m.ProductVat,
                    UnitId = m.UnitId,
                    IsAvailable = m.IsAvailable,
                    Status = m.Status,
                    ProductImages = m.ProductImages
                    .OrderBy(pi => pi.ProductImageId) 
                    .Take(1)
                    .ToList()
                })
                .ToListAsync();

            return menuList;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while retrieving the menu.", ex);
        }
    }

    //Pham Son Tung
    public async Task<IEnumerable<Menu>> GetAllMenuQr()
    {
        try
        {
            var menuList = await _context.Menus
                .AsNoTracking()
                .Include(m => m.ProductImages)
                .Where(m => (m.IsDelete != true && m.IsAvailable == true && m.Status==true))
                .OrderBy(m => m.ProductName)
                .Select(m => new Menu
                {
                    ProductId = m.ProductId,
                    ProductName = m.ProductName,
                    ProductCategoryId = m.ProductCategoryId,
                    SellPrice = m.SellPrice,
                    SalePrice = m.SalePrice,
                    ProductVat = m.ProductVat,
                    UnitId = m.UnitId,
                    IsAvailable = m.IsAvailable,
                    Status = m.Status,
                    ProductImages = m.ProductImages
                    .OrderBy(pi => pi.ProductImageId)
                    .Take(1)
                    .ToList()
                })
                .ToListAsync();

            return menuList;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while retrieving the menu.", ex);
        }
    }

    //Pham Son Tung
    //Func for api FilterProductPos 
    public async Task<IEnumerable<Menu>> FilterMenuPos(int? categoryId, bool? isAvailable)
    {
        try
        {
            var query = _context.Menus
                .AsNoTracking()
                .Where(m => m.IsDelete == false && m.Status == true);

            if (categoryId.HasValue)
                query = query.Where(m => m.ProductCategoryId == categoryId.Value);

            if (isAvailable.HasValue)
                query = query.Where(m => m.IsAvailable == isAvailable.Value);
            return await query
                .OrderBy(m => m.ProductName)
                                .Select(m => new Menu
                                {
                                    ProductId = m.ProductId,
                                    ProductName = m.ProductName,
                                    ProductCategoryId = m.ProductCategoryId,
                                    SellPrice = m.SellPrice,
                                    SalePrice = m.SalePrice,
                                    ProductVat = m.ProductVat,
                                    UnitId = m.UnitId,
                                    IsAvailable = m.IsAvailable,
                                    Status = m.Status,
                                    // Lấy ProductImage đầu tiên của mỗi sản phẩm, nếu có
                                    ProductImages = m.ProductImages.Any() ? new List<ProductImage> { m.ProductImages.FirstOrDefault() } : null
                                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error when filter from database.", ex);
        }
    }

    // Method to delete an image by ID
    public async Task<bool> DeleteImageByIdAsync(int productId)
    {

        var images = await _context.ProductImages
     .Where(pi => pi.ProductId == productId)
     .ToListAsync();

        if (images == null || !images.Any())
        {
            return false; // No images found for this product
        }

        _context.ProductImages.RemoveRange(images);
        await _context.SaveChangesAsync();
        return true; // Images deleted successfully

    }
}
