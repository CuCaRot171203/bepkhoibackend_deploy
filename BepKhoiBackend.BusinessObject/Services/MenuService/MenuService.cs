using AutoMapper;
using BepKhoiBackend.BusinessObject.Abstract.MenuBusinessAbstract;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using BepKhoiBackend.BusinessObject.dtos.RoomDto;
using BepKhoiBackend.BusinessObject.Helpers;
using BepKhoiBackend.DataAccess.Abstract.MenuAbstract;
using BepKhoiBackend.DataAccess.Helpers;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.Shared.Helpers;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BepKhoiBackend.BusinessObject.Services.MenuService
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly bepkhoiContext _context;
        private readonly IMapper _mapper;
        private readonly CloudinaryService _cloudinaryService;

        public MenuService(
            IMenuRepository menuRepository,
            bepkhoiContext context,
            IMapper mapper,
            CloudinaryService cloudinaryService)
        {
            _menuRepository = menuRepository;
            _context = context;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        // Method to get menu by filter
        public async Task<ResultWithList<Menu>> GetAllMenusAsync(
            string sortBy, string sortDirection,
            int? categoryId, bool? isActive, string? productNameOrId)
        {
            try
            {
                // Validate sortBy
                var allowedSortFields = new List<string> { "ProductId", "ProductName", "SellPrice", "CostPrice" };
                if (!allowedSortFields.Contains(sortBy))
                {
                    return new ResultWithList<Menu> { IsSuccess = false, Message = $"Invalid sort field: {sortBy}" };
                }

                var query = _menuRepository.GetMenusQueryable();

                // Filter by categoryId
                if (categoryId.HasValue)
                    query = query.Where(m => m.ProductCategoryId == categoryId.Value);

                // Filter by isActive
                if (isActive.HasValue)
                    query = query.Where(m => m.Status == isActive.Value);

                // Sorting
                query = MenuHelper.ApplySorting(query, sortBy, sortDirection);

                // Get all data from DB
                var data = await query.ToListAsync();

                // Filter by ProductId or ProductName after DB query
                if (!string.IsNullOrEmpty(productNameOrId))
                {
                    var searchValue = productNameOrId.Trim().ToLower();
                    var searchNoSign = DataAccess.Helpers.StringHelper.RemoveDiacritics(searchValue);

                    data = data.Where(m =>
                        m.ProductId.ToString() == searchValue ||
                        DataAccess.Helpers.StringHelper.RemoveDiacritics(m.ProductName.ToLower()).Contains(searchNoSign)
                    ).ToList();
                }

                return new ResultWithList<Menu>
                {
                    IsSuccess = data.Any(),
                    Message = data.Any() ? "Get all menus successfully." : "No product matched the condition.",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new ResultWithList<Menu> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        //get menu for customer
        public async Task<ResultWithList<MenuCustomerDto>> GetAllMenusCustomerAsync(
    string sortBy, string sortDirection,
    int? categoryId, bool? isActive, string? productNameOrId)
        {
            try
            {
                var allowedSortFields = new List<string> { "ProductId", "ProductName", "SellPrice", "CostPrice" };
                if (!allowedSortFields.Contains(sortBy))
                {
                    return new ResultWithList<MenuCustomerDto> { IsSuccess = false, Message = $"Invalid sort field: {sortBy}" };
                }

                var query = _menuRepository.GetMenusQueryable();
                query = query.Where(m => m.Status == true && m.IsAvailable == true);

                if (categoryId.HasValue)
                    query = query.Where(m => m.ProductCategoryId == categoryId.Value);

                if (isActive.HasValue)
                    query = query.Where(m => m.Status == isActive.Value);

                query = MenuHelper.ApplySorting(query, sortBy, sortDirection);

                var data = await query
                    .Include(m => m.ProductImages)
                    .ToListAsync();

                if (!string.IsNullOrEmpty(productNameOrId))
                {
                    var searchValue = productNameOrId.Trim().ToLower();
                    var searchNoSign = DataAccess.Helpers.StringHelper.RemoveDiacritics(searchValue);

                    data = data.Where(m =>
                        m.ProductId.ToString() == searchValue ||
                        DataAccess.Helpers.StringHelper.RemoveDiacritics(m.ProductName.ToLower()).Contains(searchNoSign)
                    ).ToList();
                }

                var mappedData = data.Select(m => new MenuCustomerDto
                {
                    ProductId = m.ProductId,
                    ProductName = m.ProductName,
                    ProductCategoryId = m.ProductCategoryId,
                    CostPrice = m.CostPrice,
                    SellPrice = m.SellPrice,
                    SalePrice = m.SalePrice,
                    ProductVat = m.ProductVat,
                    Description = m.Description,
                    UnitId = m.UnitId,
                    IsAvailable = m.IsAvailable,
                    Status = m.Status,
                    IsDelete = m.IsDelete,
                    ProductImages = m.ProductImages.Select(img => new ProductImageDto
                    {
                        ImageUrl = img.ProductImage1
                    }).ToList()
                }).ToList();

                return new ResultWithList<MenuCustomerDto>
                {
                    IsSuccess = mappedData.Any(),
                    Message = mappedData.Any() ? "Get all menus successfully." : "No product matched the condition.",
                    Data = mappedData
                };
            }
            catch (Exception ex)
            {
                return new ResultWithList<MenuCustomerDto> { IsSuccess = false, Message = $"Error: {ex.Message}" };
            }
        }

        // Method get menu by Id
        public async Task<PagedResult<Menu>> GetMenuByIdAsync(int pId)
        {
            try
            {
                var menu = await _menuRepository.GetMenuByIdAsync(pId);

                // Check null
                if (menu == null)
                {
                    return new PagedResult<Menu>
                    {
                        IsSuccess = false,
                        Message = $"Menu with ID {pId} not found or has been deleted.",
                        Data = new List<Menu>(),
                        TotalRecords = 0,
                        Page = 1,
                        PageSize = 1
                    };
                }

                return new PagedResult<Menu>
                {
                    IsSuccess = true,
                    Message = $"Found menu with ID {pId}.",
                    Data = new List<Menu> { menu },
                    TotalRecords = 1,
                    Page = 1,
                    PageSize = 1
                };
            }
            catch (ArgumentException ex)
            {
                return new PagedResult<Menu>
                {
                    IsSuccess = false,
                    Message = $"Invalid argument: {ex.Message}",
                    Data = new List<Menu>(),
                    TotalRecords = 0,
                    Page = 1,
                    PageSize = 1
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<Menu>
                {
                    IsSuccess = false,
                    Message = $"Error occurred while retrieving menu by ID: {ex.Message}",
                    Data = new List<Menu>(),
                    TotalRecords = 0,
                    Page = 1,
                    PageSize = 1
                };
            }
        }

        // Method add menu
        public async Task<PagedResult<MenuDto>> AddMenuAsync(CreateMenuDto menuDto, List<string> imageUrls)
        {
            try
            {
                if (menuDto == null)
                    throw new ArgumentException("Menu data must not be null.");

                bool isExistName = await _menuRepository.CheckMenuExistByName(menuDto.ProductName);
                if (isExistName)
                {
                    return new PagedResult<MenuDto>
                    {
                        IsSuccess = false,
                        Message = "Product name already exists.",
                        Data = new List<MenuDto>(),
                        TotalRecords = 0,
                        Page = 1,
                        PageSize = 1
                    };
                }

                // Map DTO to Entity
                var menuEntity = _mapper.Map<Menu>(menuDto);

                // Thêm danh sách ảnh vào menuEntity
                if (imageUrls != null && imageUrls.Any())
                {
                    menuEntity.ProductImages = imageUrls.Select(url => new ProductImage
                    {
                        ProductImage1 = url
                    }).ToList();
                }

                // Save to database
                var addedMenu = await _menuRepository.AddMenuAsync(menuEntity);

                var addedMenuDto = _mapper.Map<MenuDto>(addedMenu);

                return new PagedResult<MenuDto>
                {
                    IsSuccess = true,
                    Message = "Menu added successfully.",
                    Data = new List<MenuDto> { addedMenuDto },
                    TotalRecords = 1,
                    Page = 1,
                    PageSize = 1
                };
            }
            catch (ArgumentException ex)
            {
                return new PagedResult<MenuDto>
                {
                    IsSuccess = false,
                    Message = $"Invalid argument: {ex.Message}",
                    Data = new List<MenuDto>(),
                    TotalRecords = 0,
                    Page = 1,
                    PageSize = 1
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<MenuDto>
                {
                    IsSuccess = false,
                    Message = $"Error occurred while adding menu: {ex.Message}",
                    Data = new List<MenuDto>(),
                    TotalRecords = 0,
                    Page = 1,
                    PageSize = 1
                };
            }
        }


        public async Task<Result<Menu>> UpdateMenuAsync(int productId, UpdateMenuDto dto, List<string> imageUrls)
        {
            try
            {
                var existingMenu = await _menuRepository.GetMenuByIdAsync(productId);

                if (existingMenu == null)
                    return Result<Menu>.Failure($"Menu with ID {productId} not found.");

                if (existingMenu.IsDelete == true)
                    return Result<Menu>.Failure($"Menu with ID {productId} has been deleted.");

                // Cập nhật thông tin
                existingMenu.ProductName = dto.ProductName;
                existingMenu.ProductCategoryId = dto.ProductCategoryId;
                existingMenu.CostPrice = dto.CostPrice;
                existingMenu.SellPrice = dto.SellPrice;
                existingMenu.SalePrice = dto.SalePrice;
                existingMenu.ProductVat = dto.ProductVat;
                existingMenu.Description = dto.Description;
                existingMenu.UnitId = dto.UnitId;
                existingMenu.IsAvailable = dto.IsAvailable ?? existingMenu.IsAvailable;
                existingMenu.Status = dto.Status ?? existingMenu.Status;

                // Cập nhật ảnh
                if (imageUrls.Any())
                {
                    await _menuRepository.DeleteImageByIdAsync(productId); // Xóa record cũ trong DB

                    existingMenu.ProductImages = imageUrls.Select(url => new ProductImage
                    {
                        ProductImage1 = url
                    }).ToList();
                }

                var updatedMenu = await _menuRepository.UpdateMenuAsync(existingMenu);

                return Result<Menu>.Success(updatedMenu, $"Menu with ID {productId} updated successfully.");
            }
            catch (ArgumentException ex)
            {
                return Result<Menu>.Failure($"Invalid argument: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result<Menu>.Failure($"Error occurred while updating menu: {ex.Message}");
            }
        }


        // Method to soft delete
        // Method to soft delete
        public async Task<PagedResult<Menu>> DeleteMenuAsync(int id)
        {
            try
            {
                var menu = await _menuRepository.GetMenuByIdAsync(id);

                // Check null
                if (menu == null)
                {
                    return new PagedResult<Menu>
                    {
                        IsSuccess = false,
                        Message = $"Menu with ID {id} not found.",
                        Data = new List<Menu>(),
                        TotalRecords = 0
                    };
                }

                // Check soft delete
                if (menu.IsDelete == true)
                {
                    return new PagedResult<Menu>
                    {
                        IsSuccess = false,
                        Message = $"Menu with ID {id} has already been deleted.",
                        Data = new List<Menu>(),
                        TotalRecords = 0
                    };
                }

                // Delete associated images from Cloudinary and database
                if (menu.ProductImages != null && menu.ProductImages.Any())
                {
                    foreach (var image in menu.ProductImages)
                    {
                        await _cloudinaryService.DeleteImageAsync(image.ProductImage1);
                    }
                    menu.ProductImages.Clear();
                }
                await _menuRepository.DeleteImageByIdAsync(id);
                // Perform soft delete
                await _menuRepository.DeleteMenuAsync(menu);

                return new PagedResult<Menu>
                {
                    IsSuccess = true,
                    Message = $"Menu with ID {id} deleted successfully.",
                    Data = new List<Menu> { menu },
                    TotalRecords = 1
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<Menu>
                {
                    IsSuccess = false,
                    Message = $"Error occurred while deleting menu: {ex.Message}",
                    Data = new List<Menu>(),
                    TotalRecords = 0
                };
            }
        }

        // Method to export product excel file
        public async Task<(byte[] fileContent, string FileName, bool HasData, string ErrorMessage)> ExportActiveProductsToExcelAsync(
        string sortBy, string sortDirection, int? categoryId = null, bool? isActive = null)
        {
            try
            {
                // Validate sortBy, sortDirection
                if (!MenuHelper.ValidateSortParams(sortBy, sortDirection))
                    return (null, null, false, "Invalid sort parameters. Please check column name and direction (asc/desc).");

                // Check category
                if (categoryId.HasValue)
                {
                    bool isCategoryExist = await _context.ProductCategories.AnyAsync(c => c.ProductCategoryId == categoryId.Value);
                    if (!isCategoryExist)
                        return (null, null, false, $"Category ID {categoryId.Value} does not exist.");
                }

                var products = await _menuRepository.GetMenusByConditionAsync(categoryId, sortBy, sortDirection);

                // Apply isActive filter
                if (isActive.HasValue)
                    products = products.Where(m => MenuHelper.FilterProductByTypeOfIsActive(m) == isActive.Value).ToList();

                // Check has data
                if (!products.Any())
                    return (null, null, false, "No product data found to export.");

                var now = DateTime.Now;

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Products List");

                    // Title
                    var title = $"List of products by day {now:dd/MM/yyyy}";
                    worksheet.Range("A1:K1").Merge().Value = title;
                    var titleCell = worksheet.Cell("A1");
                    titleCell.Style.Font.Bold = true;
                    titleCell.Style.Font.FontSize = 16;
                    titleCell.Style.Font.FontColor = XLColor.White;
                    titleCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#535bed");
                    titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Header
                    var headers = new[] { "Product ID", "Product Name", "Category ID", "Cost Price", "Sell Price", "Sale Price", "VAT", "Description", "Unit ID", "Available", "Status" };
                    for (int i = 0; i < headers.Length; i++) worksheet.Cell(2, i + 1).Value = headers[i];
                    var headerRange = worksheet.Range("A2:K2");
                    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#535bed");
                    headerRange.Style.Font.FontColor = XLColor.White;
                    headerRange.Style.Font.Bold = true;

                    // Data
                    int row = 3;
                    foreach (var p in products)
                    {
                        worksheet.Cell(row, 1).Value = p.ProductId;
                        worksheet.Cell(row, 2).Value = p.ProductName;
                        worksheet.Cell(row, 3).Value = p.ProductCategoryId;
                        worksheet.Cell(row, 4).Value = p.CostPrice;
                        worksheet.Cell(row, 5).Value = p.SellPrice;
                        worksheet.Cell(row, 6).Value = p.SalePrice;
                        worksheet.Cell(row, 7).Value = p.ProductVat;
                        worksheet.Cell(row, 8).Value = p.Description;
                        worksheet.Cell(row, 9).Value = p.UnitId;
                        worksheet.Cell(row, 10).Value = p.IsAvailable == true ? "Yes" : "No";
                        worksheet.Cell(row, 11).Value = p.Status == true ? "Active" : "Inactive";
                        row++;
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var fileContent = stream.ToArray();
                        string fileName = $"{(isActive == true ? "Active" : (isActive == false ? "Inactive" : "All"))}_ProductList_{now:dd_MM_yyyy}" +
                                          $"{(categoryId.HasValue ? $"_ByCategory_{categoryId.Value}" : "_No_Filter")}.xlsx";

                        return (fileContent, fileName, true, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return (null, null, false, $"Unexpected error: {ex.Message}");
            }
        }

        // Method to export product price excel file
        public async Task<(byte[] fileContent, string FileName, bool HasData, string ErrorMessage)> ExportPriceExcelAsync(
            string sortBy, string sortDirection, int? categoryId = null, bool? isActive = null)
        {
            try
            {
                // Validate sort params
                if (!MenuHelper.ValidateSortParams(sortBy, sortDirection))
                    return (null, null, false, "Invalid sort parameters. Please check column name and direction (asc/desc).");

                var query = await _menuRepository.GetFilteredMenusAsync(categoryId);
                query = MenuHelper.ApplySorting(query, sortBy, sortDirection);
                var products = await query.ToListAsync();

                // Check value
                if (isActive.HasValue)
                {
                    products = products
                        .Where(m => MenuHelper.FilterProductByTypeOfIsActive(m) == isActive.Value)
                        .ToList();
                }

                // Check data
                if (!products.Any())
                    return (null, null, false, "No product price data found to export.");

                var fileContent = ExcelExportHelper.GenerateProductPriceExcel(products);
                var fileName = $"ProductPriceList_{DateTime.Now:dd_MM_yyyy}.xlsx";

                return (fileContent, fileName, true, null);
            }
            catch (ArgumentException ex)
            {
                return (null, null, false, ex.Message);
            }
            catch (Exception ex)
            {
                return ExcelExportHelper.HandleExcelExportException(ex);
            }
        }

        // Method update price of product 
        public async Task<(bool IsSuccess, string Message, Menu Data)> UpdatePriceOfProductAsync(UpdatePriceDto dto)
        {
            
            var (isValid, errorMessage) = MenuDtoHelper.ValidateUpdatePriceDto(dto);
            if (!isValid) return (false, errorMessage, null);

            // Check Exist
            if (!await _menuRepository.CheckMenuExistById(dto.ProductId))
                return (false, $"Product with ID {dto.ProductId} does not exist.", null);
            if (await _menuRepository.CheckMenuIsDelete(dto.ProductId))
                return (false, $"Product with ID {dto.ProductId} has been deleted and cannot be updated.", null);

            var existingMenu = await _menuRepository.GetMenuByIdForUpdatePriceAsync(dto.ProductId);
            if (existingMenu == null)
                return (false, $"Unexpected error: Product with ID {dto.ProductId} could not be found.", null);

            // Update price
            existingMenu.CostPrice = dto.CostPrice;
            existingMenu.SellPrice = dto.SellPrice;
            existingMenu.SalePrice = dto.SalePrice;
            existingMenu.ProductVat = dto.ProductVat;

            // Call repo
            await _menuRepository.UpdateMenuPriceAsync(existingMenu);

            return (true, $"Product price with ID {dto.ProductId} updated successfully.", existingMenu);
        }

        //Pham Son Tung
        //Func for api GetAllMenuPos
        public async Task<IEnumerable<MenuPosDto>> GetAllMenuPosAsync()
        {
            try
            {
                // Lấy danh sách menu từ repository
                var menuList = await _menuRepository.GetAllMenuPos();

                // Map từ Menu entity sang MenuDto
                var menuDtoList = menuList.Select(m => new MenuPosDto
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
                    ProductImageUrl = m.ProductImages==null?null:m.ProductImages.FirstOrDefault()?.ProductImage1,
                }).ToList();

                return menuDtoList;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving the menu in service layer.", ex);
            }
        }
        //Pham Son Tung
        public async Task<IEnumerable<MenuQrDto>> GetAllMenuQrAsync()
        {
            try
            {
                // Lấy danh sách menu từ repository
                var menuList = await _menuRepository.GetAllMenuQr();

                // Map từ Menu entity sang MenuPosDto
                var menuDtoList = menuList.Select(m => new MenuQrDto
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
                    ProductImageUrls = m.ProductImages?.Select(img => img.ProductImage1).ToList()
                }).ToList();

                return menuDtoList;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving the menu in service layer.", ex);
            }
        }


        //Pham Son Tung
        //Func for FilterProductPos API
        public async Task<List<MenuPosDto>> FilterMenuAsyncPos(int? categoryId, bool? isAvailable)
        {

            if (categoryId <= 0)
            {
                throw new ArgumentOutOfRangeException("ProductCategoryId must greater than 0");
            }

            var menuList = await _menuRepository.FilterMenuPos(categoryId, isAvailable);

            var menuDtoList = menuList.Select(m => new MenuPosDto
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
                ProductImageUrl = m.ProductImages == null ? null : m.ProductImages.FirstOrDefault().ProductImage1,
            }).ToList();

            return menuDtoList;
        }


        //Create, update, delete product category function
        public Task AddProductCategoryAsync(string productCategoryTitle)
        {
            return _menuRepository.AddProductCategoryAsync(productCategoryTitle);
        }

        public Task UpdateProductCategoryAsync(int productCategoryId, string productCategoryTitle)
        {
            return _menuRepository.UpdateProductCategoryAsync(productCategoryId, productCategoryTitle);
        }

        public Task SoftDeleteProductCategoryAsync(int productCategoryId)
        {
            return _menuRepository.SoftDeleteProductCategoryAsync(productCategoryId);
        }
    }
}
