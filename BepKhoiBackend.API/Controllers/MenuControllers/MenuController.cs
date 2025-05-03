using AutoMapper;
using BepKhoiBackend.DataAccess.Abstract.MenuAbstract;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BepKhoiBackend.BusinessObject.Abstract.MenuBusinessAbstract;
using BepKhoiBackend.BusinessObject.dtos.RoomDto;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace BepKhoiBackend.API.Controllers.MenuControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;
        private readonly IMenuService _menuService;
        private readonly ILogger<MenuController> _logger;
        private readonly CloudinaryService _cloudinaryService;

        public MenuController(
            CloudinaryService cloudinaryService,
            IMenuRepository menuRepository,
            IMapper mapper,
            ILogger<MenuController> logger,
            IMenuService menuService)
        {
            _menuRepository = menuRepository;
            _menuService = menuService;
            _mapper = mapper;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
        }


        /*========== NEW MENU API CONTROLLER =======*/
        // API - MenuController.cs
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("get-all-menus")]
        public async Task<IActionResult> GetAllMenuAsync(
            [FromQuery] string sortBy = "ProductId",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] int? categoryId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? productNameOrId = null)
        {
            try
            {
                var result = await _menuService.GetAllMenusAsync(sortBy, sortDirection, categoryId, isActive, productNameOrId);
                if (!result.IsSuccess)
                    return NotFound(new { message = result.Message });

                var mappedData = _mapper.Map<IEnumerable<MenuDto>>(result.Data);

                return Ok(new
                {
                    message = result.Message,
                    data = mappedData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("get-all-menus-customer")]
        public async Task<IActionResult> GetAllMenuCustomerAsync(
        [FromQuery] string sortBy = "ProductId",
        [FromQuery] string sortDirection = "asc",
        [FromQuery] int? categoryId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? productNameOrId = null)
        {
            try
            {
                var result = await _menuService.GetAllMenusCustomerAsync(sortBy, sortDirection, categoryId, isActive, productNameOrId);
                if (!result.IsSuccess)
                    return NotFound(new { message = result.Message });

                return Ok(new
                {
                    message = result.Message,
                    data = result.Data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Roles = "manager, cashier")]
        // API Get menu by ID
        [HttpGet("get-menu-by-id/{pid}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(404)] // NotFound
        [ProducesResponseType(500)] // InternalServerError
        public async Task<IActionResult> GetMenuById(int pid)
        {
            try
            {
                if (pid <= 0)
                {
                    return BadRequest(new { message = "Product ID must be greater than 0." });
                }

                var result = await _menuService.GetMenuByIdAsync(pid);

                if (!result.IsSuccess || result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning($"Couldn't find menu with ID: {pid}");
                    return NotFound(new { message = result.Message });
                }
                var menudata = result.Data.FirstOrDefault();
                var mappedData = new MenuForManagerDto
                {
                    ProductId = menudata.ProductId,
                    ProductName = menudata.ProductName,
                    ProductCategoryId = menudata.ProductCategoryId,
                    CostPrice = menudata.CostPrice,
                    SellPrice = menudata.SellPrice,
                    SalePrice = menudata.SalePrice,
                    ProductVat = menudata.ProductVat,
                    Description = menudata.Description,
                    UnitId = menudata.UnitId,
                    IsAvailable= menudata.IsAvailable,
                    Status = menudata.Status,
                    IsDelete = menudata.IsDelete,
                    ImageUrl = menudata.ProductImages.FirstOrDefault()?.ProductImage1 ?? string.Empty
                };

                return Ok(new
                {
                    message = result.Message,
                    data = mappedData
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving the menu." });
            }
        }

        // API add product to Menu list
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpPost("add")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddMenu([FromForm] CreateMenuDto menuDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid menu data received.");
                    return BadRequest(ModelState);
                }
                var imageUrls = new List<string>();
                if (menuDto.Image != null)
                {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(menuDto.Image);
                    imageUrls.Add(imageUrl);
                }
               
                // Call service
                var result = await _menuService.AddMenuAsync(menuDto, imageUrls);

                // Upload images to Cloudinary

                if (!result.IsSuccess)
                {
                    _logger.LogWarning($"Failed to add menu: {result.Message}");
                    return BadRequest(new { message = result.Message });
                }

                var addedMenuDto = result.Data.First();

                // Return result successfully
                return CreatedAtAction(
                    nameof(GetMenuById),
                    new { pId = addedMenuDto.ProductId },
                    new
                    {
                        message = result.Message,
                        data = addedMenuDto
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding the menu.");
                return StatusCode(500, new { message = "An unexpected error occurred while adding the menu." });
            }
        }


        [Authorize(Roles = "manager")]
        [HttpPut("update-menu/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateMenu(int id, [FromForm] UpdateMenuDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new { message = "Validation failed", errors });
                }

                if (id <= 0)
                    return BadRequest(new { message = "Product ID must be greater than 0." });

                var menuResult = await _menuService.GetMenuByIdAsync(id);
                var existingMenu = menuResult.Data?.FirstOrDefault();
                if (existingMenu == null)
                    return NotFound(new { message = $"Menu with ID {id} not found." });
                if (existingMenu.IsDelete == true)
                    return BadRequest(new { message = $"Menu with ID {id} has been deleted." });
                string? image = existingMenu.ProductImages?.FirstOrDefault()?.ProductImage1;
                // Handle images
                List<string> imageUrls = new List<string>();
                if (dto.Image != null)
                {
                    var newImageUrl = await _cloudinaryService.UploadImageAsync(dto.Image);
                    imageUrls.Add(newImageUrl);
                }
                //if (image != null)
                //{
                //    await _cloudinaryService.DeleteImageAsync(image);
                //}
                if (dto.Image == null)
                {
                    // Keep old images
                    if (existingMenu.ProductImages != null && existingMenu.ProductImages.Any())
                    {
                        imageUrls.Add(existingMenu.ProductImages.First().ProductImage1);
                    }
                }

                var result = await _menuService.UpdateMenuAsync(id, dto, imageUrls);

                if (!result.IsSuccess)
                {
                    if (result.Message.Contains("not found"))
                        return NotFound(new { message = result.Message });
                    return BadRequest(new { message = result.Message });
                }

                var mappedData = _mapper.Map<MenuDto>(result.Data);

                return Ok(new
                {
                    message = result.Message,
                    data = mappedData
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while updating the menu." });
            }
        }


        // API to delete product
        [Authorize]
        [Authorize(Roles = "manager")]
        [HttpDelete("{productId}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> DeleteMenu(int productId)
        {
            try
            {
                var result = await _menuService.DeleteMenuAsync(productId);

                if (!result.IsSuccess)
                {
                    if (result.Message.Contains("not found") || result.Message.Contains("already been deleted"))
                    {
                        _logger.LogWarning(result.Message);
                        return NotFound(new { message = result.Message });
                    }

                    _logger.LogError(result.Message);
                    return StatusCode(500, new { message = result.Message });
                }

                return Ok(new { message = result.Message }); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when deleting menu with ID: {productId}");
                return StatusCode(500, new { message = "Error when deleting menu." });
            }
        }

        // API to export products list to excel
        [Authorize]
        [Authorize(Roles = "manager")]
        [HttpGet("export-products-excel")]
        public async Task<IActionResult> ExportProductsToExcel(
            [FromQuery] string sortBy = "ProductId",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] int? categoryId = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var (fileContent, fileName, hasData, errorMessage) = await _menuService.ExportActiveProductsToExcelAsync(sortBy, sortDirection, categoryId, isActive);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    if (errorMessage.Contains("does not exist")) return BadRequest(new { message = errorMessage });
                    if (errorMessage.Contains("No product data found")) return NotFound(new { message = errorMessage });
                    return StatusCode(500, new { message = errorMessage });
                }

                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }


        // API to export products price list to excel
        [Authorize]
        [Authorize(Roles = "manager")]
        [HttpGet("export-product-price-excel")]
        public async Task<IActionResult> ExportProductPriceToExcel(
            [FromQuery] string sortBy = "ProductId",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] int? categoryId = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var (fileContent, fileName, hasData, errorMessage) = await _menuService.ExportPriceExcelAsync(sortBy, sortDirection, categoryId, isActive);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    if (errorMessage.Contains("does not exist"))
                        return BadRequest(new { message = errorMessage });
                    if (errorMessage.Contains("No product price data found"))
                        return NotFound(new { message = errorMessage });
                    return StatusCode(500, new { message = errorMessage });
                }
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        // API to update price of product
        [Authorize]
        [Authorize(Roles = "manager")]
        [HttpPut("update-price/{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePriceOfProduct(int productId, [FromBody] UpdatePriceDto priceDto)
        {
            try
            {
                if (priceDto == null || productId != priceDto.ProductId)
                {
                    _logger.LogWarning("Product ID does not match or data is invalid.");
                    return BadRequest(new { message = "Product ID does not match or data is invalid." });
                }

                var (isSuccess, message, data) = await _menuService.UpdatePriceOfProductAsync(priceDto);

                if (!isSuccess)
                {
                    _logger.LogWarning(message);
                    if (message.Contains("does not exist") || message.Contains("deleted"))
                        return NotFound(new { message });
                    return BadRequest(new { message });
                }

                return Ok(new { message = "Update product price successfully.", data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when updating product price with ID: {productId}");
                return StatusCode(500, new { message = "Internal server error while updating product price." });
            }
        }




        //Pham Son Tung
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("get-menu-pos")]
        public async Task<IActionResult> GetMenu()
        {
            try
            {
                // Gọi service để lấy danh sách món ăn
                var menuList = await _menuService.GetAllMenuPosAsync();

                // Kiểm tra nếu không có món ăn nào
                if (menuList == null || !menuList.Any())
                {
                    return NotFound(new { success = false, message = "No menu items found." });
                }

                // Trả về kết quả thành công với danh sách món ăn
                return Ok(new { success = true, data = menuList });
            }
            catch (InvalidOperationException ex)
            {
                // Xử lý lỗi từ service layer
                return StatusCode(500, new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the menu.", details = ex.Message });
            }
        }

        //Pham Son Tung
        // controller for filter by roomAreaId and isUse
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("filter-menu-pos")]
        [ProducesResponseType(typeof(List<MenuPosDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FilterMenuPos([FromQuery] int? categoryId, [FromQuery] bool? isAvailable)
        {
            try
            {
                var result = await _menuService.FilterMenuAsyncPos(categoryId, isAvailable);
                if (result == null || !result.Any())
                {
                    return NotFound(new { message = "Can't find data of product." });
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error server.", error = ex.Message });
            }
        }


        //Pham Son Tung
        [HttpGet("get-all-menu-qr")]
        public async Task<IActionResult> GetAllMenuQr()
        {
            try
            {
                IEnumerable<MenuQrDto> menus = await _menuService.GetAllMenuQrAsync();

                return Ok(menus);
            }
            catch (InvalidOperationException ex)
            {
                // Log lỗi nếu cần
                return StatusCode(500, new
                {
                    message = "Đã xảy ra lỗi khi lấy danh sách thực đơn.",
                    detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                // Log lỗi không xác định
                return StatusCode(500, new
                {
                    message = "Đã xảy ra lỗi không xác định.",
                    detail = ex.Message
                });
            }
        }


        //Create, update, delete product category function
        [HttpPost("product-category")]
        public async Task<IActionResult> AddProductCategory([FromQuery] string title)
        {
            try
            {
                await _menuService.AddProductCategoryAsync(title);
                return Ok(new { message = "Thêm danh mục thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi nội bộ.", detail = ex.Message });
            }
        }

        [HttpPut("product-category")]
        public async Task<IActionResult> UpdateProductCategory([FromQuery] int id, [FromQuery] string title)
        {
            try
            {
                await _menuService.UpdateProductCategoryAsync(id, title);
                return Ok(new { message = "Cập nhật danh mục thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi nội bộ.", detail = ex.Message });
            }
        }

        [HttpDelete("product-category/{id}")]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            try
            {
                await _menuService.SoftDeleteProductCategoryAsync(id);
                return Ok(new { message = "Xóa mềm danh mục thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi nội bộ.", detail = ex.Message });
            }
        }
    }
}
