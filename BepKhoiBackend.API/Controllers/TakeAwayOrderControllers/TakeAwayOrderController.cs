using BepKhoiBackend.BusinessObject.Services.TakeAwayOrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BepKhoiBackend.API.Controllers.TakeAwayOrderControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TakeAwayOrderController : ControllerBase
    {
        private readonly ITakeAwayOrderService _takeAwayOrderService;

        public TakeAwayOrderController(ITakeAwayOrderService takeAwayOrderService)
        {
            _takeAwayOrderService = takeAwayOrderService;
        }

        [Authorize(Roles = "manager, cashier")]
        [HttpGet("takeaway")]
        public IActionResult GetTakeAwayOrders()
        {
            var orders = _takeAwayOrderService.GetTakeAwayOrders();
            return Ok(orders);
        }
    }
}
