using BepKhoiBackend.API.Hubs;
using BepKhoiBackend.BusinessObject.Abstract.OrderAbstract;
using BepKhoiBackend.BusinessObject.Abstract.OrderDetailAbstract;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.BusinessObject.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace BepKhoiBackend.API.Controllers.OrderDetailControllers
{
    [ApiController]
    [Route("api/order-detail")]
    public class OrderDetailControllers : ControllerBase
    {
        private readonly IHubContext<SignalrHub> _hubContext;
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailControllers(IOrderDetailService orderDetailService, IHubContext<SignalrHub> hubContext)
        {
            _hubContext = hubContext;
            _orderDetailService = orderDetailService;
        }

        [HttpGet("get-by-order-id/{orderId}")]
        public async Task<IActionResult> GetOrderDetailsByOrderId(int orderId)
        {
            var result = await _orderDetailService.GetOrderDetailsByOrderIdAsync(orderId);

            if (!result.IsSuccess)
                return NotFound(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
        }

        [HttpDelete("cancel-order-detail")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(404)] // NotFound
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> CancelOrderDetail([FromBody] CancelOrderDetailRequest request)
        {
            try
            {
                if (request.OrderId <= 0 || request.OrderDetailId <= 0 || request.CashierId <= 0 || request.Quantity <= 0)
                {
                    return BadRequest(new { message = "Invalid input parameters." });
                }

                var result = await _orderDetailService.CancelOrderDetailAsync(request);
                return Ok(new { message = "Order detail canceled successfully", data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        // API to remove order detail
        [HttpDelete("remove-order-detail")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(404)] // NotFound
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> RemoveOrderDetail([FromBody] RemoveOrderDetailRequest request)
        {
            try
            {
                if (request.OrderId <= 0 || request.OrderDetailId <= 0)
                {
                    return BadRequest(new { message = "Invalid Order ID or Order Detail ID." });
                }

                var result = await _orderDetailService.RemoveOrderDetailAsync(request);
                if (!result)
                {
                    return NotFound(new { message = "Order detail not found or cannot be removed." });
                }

                return Ok(new { message = "Order detail removed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        [HttpPut("add-note-to-order-detail")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> AddNoteToOrderDetail([FromBody] AddNoteToOrderDetailRequest request)
        {
            try
            {
                if (request.OrderId <= 0 || request.OrderDetailId <= 0 || string.IsNullOrWhiteSpace(request.Note))
                {
                    return BadRequest(new { message = "Invalid input parameters." });
                }

                var result = await _orderDetailService.AddNoteToOrderDetailAsync(request);
                if (!result)
                {
                    return NotFound(new { message = "Order detail not found." });
                }

                return Ok(new { message = "Note added successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        //Pham Son Tung
        //Api ConfirmOrderPos of POS site 
        [Authorize(Roles = "manager, cashier")]
        [HttpPut("confirm/{orderId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ConfirmOrderPos([FromRoute] int orderId)
        {
            try
            {
                var success = await _orderDetailService.ConfirmOrderPosServiceAsync(orderId);

                if (!success)
                {
                    return NotFound(new { message = "No order details found. Order may not exist." });
                }
                await _hubContext.Clients.Group("order").SendAsync("OrderUpdate", orderId);
                return Ok(new { message = "Order confirmed successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        //Pham Son Tung
        //Api SplitOrderPos of POS site
        [Authorize(Roles = "manager, cashier")]
        [HttpPost("SplitOrderPos")]
        [ProducesResponseType(typeof(object), 200)] // Thành công
        [ProducesResponseType(typeof(object), 400)] // Bad Request
        [ProducesResponseType(typeof(object), 500)] // Internal Server Error
        public async Task<IActionResult> SplitOrderPos([FromBody] SplitOrderPosRquest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Invalid request data." });
            }

            try
            {
                bool result = await _orderDetailService.SplitOrderPosServiceAsync(request);
                if (result)
                {
                    await _hubContext.Clients.Group("order").SendAsync("OrderUpdate", request.OrderId);
                    if (request.SplitTo.HasValue)
                    {
                        await _hubContext.Clients.Group("order").SendAsync("OrderUpdate", request.SplitTo);
                    }
                    return Ok(new { message = "Order split successfully." });
                }
                return BadRequest(new { message = "Failed to split order." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Database update failed.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }
    }
}
