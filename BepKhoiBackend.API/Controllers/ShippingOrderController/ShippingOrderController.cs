using BepKhoiBackend.BusinessObject.Services.ShippingOrderService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BepKhoiBackend.API.Controllers.ShippingOrderController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingOrderController : ControllerBase
    {
        private readonly IShippingOrderService _shippingOrderService;

        public ShippingOrderController(IShippingOrderService shippingOrderService)
        {
            _shippingOrderService = shippingOrderService;
        }

        [HttpGet("shippers")]
        public IActionResult GetShippersWithOrders()
        {
            var shippers = _shippingOrderService.GetShippersWithOrders();
            return Ok(shippers);
        }
    }
}
