using BepKhoiBackend.DataAccess.Models;
using System.Linq;

namespace BepKhoiBackend.DataAccess.Helpers
{
    public static class MenuHelper
    {
        private static readonly List<string> AllowedSortColumns = new List<string>
        {
            "ProductId", "ProductName", "ProductCategoryId", "CostPrice", "SellPrice", "SalePrice", "ProductVat", "Description", "UnitId", "IsAvailable", "Status"
        };


        // Function validate sortBy and sortDirection
        public static bool ValidateSortParams(string sortBy, string sortDirection)
        {
            var allowedDirections = new[] { "asc", "desc" };
            return AllowedSortColumns.Contains(sortBy, StringComparer.OrdinalIgnoreCase) &&
                   allowedDirections.Contains(sortDirection.ToLower());
        }

        // Apply Sorting - could change
        public static IQueryable<Menu> ApplySorting(IQueryable<Menu> query, string sortBy, string sortDirection)
        {
            // Default is sort by productId
            sortBy = sortBy?.ToLower() ?? "productid";
            sortDirection = sortDirection?.ToLower() ?? "desc";

            return (sortBy, sortDirection) switch
            {
                ("productname", "asc") => query.OrderBy(m => m.ProductName),
                ("productname", "desc") => query.OrderByDescending(m => m.ProductName),
                ("sellprice", "asc") => query.OrderBy(m => m.SellPrice),
                ("sellprice", "desc") => query.OrderByDescending(m => m.SellPrice),
                ("productid", "asc") => query.OrderBy(m => m.ProductId),
                ("productid", "desc") => query.OrderByDescending(m => m.ProductId),
                _ => query.OrderBy(m => m.ProductId)
            };
        }

        // Filter product by IsActive
        public static bool FilterProductByTypeOfIsActive(Menu menu)
        {
            try
            {
                if (menu == null)
                    throw new ArgumentNullException(nameof(menu), "Product (menu) is null.");

                if (menu.IsDelete.HasValue && menu.IsDelete.Value)
                    throw new InvalidOperationException($"Product with ID {menu.ProductId} has been deleted.");

                if (menu.IsAvailable.HasValue && !menu.IsAvailable.Value)
                    throw new InvalidOperationException($"Product with ID {menu.ProductId} is marked as unavailable.");

                if (menu.Status.HasValue && !menu.Status.Value)
                    throw new InvalidOperationException($"Product with ID {menu.ProductId} is inactive.");

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
