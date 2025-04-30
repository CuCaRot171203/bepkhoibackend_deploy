using BepKhoiBackend.BusinessObject.Abstract.ProductCategoryAbstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BepKhoiBackend.API.Controllers.ProductCategoryControllers
{
    [Route("api/product-categories")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoryController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [HttpGet("get-all-categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _productCategoryService.GetProductCategoriesAsync();
            return Ok(categories);
        }
    }
}
