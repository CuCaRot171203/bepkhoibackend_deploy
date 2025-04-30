using BepKhoiBackend.API.Hubs;
using BepKhoiBackend.BusinessObject.Abstract.OrderAbstract;
using BepKhoiBackend.BusinessObject.dtos.CustomerDto;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDto;
using BepKhoiBackend.BusinessObject.dtos.ShippingOrderDto;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data.Common;

namespace BepKhoiBackend.API.Controllers.OrderControllers
{

    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IHubContext<SignalrHub> _hubContext;
        private readonly IOrderService _orderService;
        private readonly PrintOrderPdfService _printOrderPdfService;
        public OrderController(IHubContext<SignalrHub> hubContext, IOrderService orderService, PrintOrderPdfService printOrderPdfService)
        {
            _hubContext = hubContext;
            _orderService = orderService;
            _printOrderPdfService = printOrderPdfService;
        }
        //get all
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("get-all-orders")]
        public async Task<IActionResult> GetAllOrdersAsync()
        {
            var result = await _orderService.GetAllOrdersAsync();

            if (!result.IsSuccess)
                return NotFound(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
        }

        [Authorize(Roles = "manager, cashier")]
        [HttpGet("filter-by-date-and-order-id")]
        public async Task<IActionResult> FilterOrdersByDateAsync(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int? orderId = null)
        {
            // Validate input parameters
            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
            {
                return BadRequest(new { message = "From date cannot be later than to date" });
            }

            if (orderId.HasValue && orderId <= 0)
            {
                return BadRequest(new { message = "Order ID must be a positive integer" });
            }

            var result = await _orderService.FilterOrdersByDateAsync(fromDate, toDate, orderId);
            return Ok(new
            {
                message = result.Message,
                data = result.Data,
                count = result.Data?.Count ?? 0
            });
        }

        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        // Create order
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateNewOrder([FromBody] CreateOrderRequestDto request)
        {
            try
            {
                var (result, roomId, isUse) = await _orderService.CreateNewOrderAsync(request);

                // Gửi sự kiện cập nhật phòng nếu có
                if (roomId.HasValue && isUse.HasValue)
                {
                    await _hubContext.Clients.Group("room").SendAsync("RoomStatusUpdate", new
                    {
                        roomId = roomId.Value,
                        isUse = isUse.Value
                    });
                }

                // Gửi sự kiện cập nhật danh sách đơn
                await _hubContext.Clients.Group("order").SendAsync("OrderListUpdate", new
                {
                    roomId = result.RoomId,
                    shipperId = result.ShipperId,
                    orderStatusId = result.OrderStatusId
                });

                return Ok(new { message = "Order created successfully", data = result });
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

        // Method to add note to OrderId
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpPut("add-note")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(404)] // NotFound
        [ProducesResponseType(500)] // Server Error
        public async Task<IActionResult> AddNoteToOrder([FromBody] AddNoteRequest request)
        {
            try
            {
                // check order Id valid
                if (request.OrderId <= 0)
                {
                    return BadRequest(new { message = "Invalid Order ID." });
                }

                var result = await _orderService.AddOrderNoteToOrderPosAsync(request);
                return Ok(new { message = "Note added successfully", data = result });
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

        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpPut("update-order-detail-quantity")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(404)] // NotFound
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> UpdateOrderDetailQuantity([FromBody] UpdateOrderDetailQuantityRequest request)
        {
            try
            {
                if (request.OrderId <= 0 || request.OrderDetailId <= 0)
                {
                    return BadRequest(new { message = "Invalid Order ID or Order Detail ID." });
                }

                var result = await _orderService.UpdateOrderDetailQuantiyPosAsync(request);
                await _hubContext.Clients.Group("order").SendAsync("OrderUpdate", request.OrderId);
                return Ok(new { message = "Order detail updated successfully", data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
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

        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpPost("add-customer-to-order")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> AddCustomerToOrderPosSite([FromBody] AddCustomerToOrderRequest request)
        {
            try
            {
                if (request.OrderId <= 0 || request.CustomerId <= 0)
                {
                    return BadRequest(new { message = "Invalid input parameters." });
                }

                var result = await _orderService.AddCustomerToOrderAsync(request);

                return Ok(new { message = "Customer added to order successfully", data = result });
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

        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpPost("add-product-to-order")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> AddProductToOrderPosSite([FromBody] AddProductToOrderRequest request)
        {
            try
            {
                if (request.OrderId <= 0 || request.ProductId <= 0)
                {
                    return BadRequest(new { message = "Invalid input parameters." });
                }

                var result = await _orderService.AddProductToOrderAsync(request);
                await _hubContext.Clients.Group("order").SendAsync("OrderUpdate", request.OrderId);
                return Ok(new { message = "Product added to order successfully", data = result });
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


        //Pham Son Tung
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpPut("MoveOrderPos")]
        public async Task<IActionResult> UpdateOrderType([FromBody] MoveOrderPosRequestDto request)
        {
            try
            {
                bool result = await _orderService.ChangeOrderTypeServiceAsync(request);
                return result
                    ? Ok(new { message = "Order type updated successfully." })
                    : BadRequest(new { message = "Failed to update order type." });
            }
            catch (ArgumentException ex) // Lỗi do tham số đầu vào không hợp lệ (service)
            {
                return BadRequest(new { message = "Invalid input parameters.", error = ex.Message });
            }
            catch (KeyNotFoundException ex) // Lỗi do không tìm thấy Order, Room hoặc User (repo)
            {
                return NotFound(new { message = "Resource not found.", error = ex.Message });
            }
            catch (Exception ex) // Các lỗi khác (bao gồm lỗi ở repository)
            {
                return StatusCode(500, new { message = "An internal server error occurred.", error = ex.Message });
            }
        }

        //Pham Son Tung
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpPut("combine-orders")]
        public async Task<IActionResult> CombineOrderPosAsync([FromBody] CombineOrderPosRequestDto request)
        {
            try
            {
                bool result = await _orderService.CombineOrderPosServiceAsync(request);
                if (result)
                {
                    await _hubContext.Clients.Group("order").SendAsync("OrderUpdate", request.FirstOrderId);
                    await _hubContext.Clients.Group("order").SendAsync("OrderUpdate", request.SecondOrderId);
                }
                return result
                    ? Ok(new { message = "Orders combined successfully." })
                    : BadRequest(new { message = "Failed to combine orders." });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An internal server error occurred.", error = ex.Message });
            }
        }

        //Pham Son Tung
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("get-order-by-type-pos")]
        public async Task<IActionResult> GetOrdersByTypePosAsync(int? roomId, int? shipperId, int? orderTypeId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByTypePosAsync(roomId, shipperId, orderTypeId);
                return Ok(orders);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(new
                {
                    message = $"Invalid parameter: {argEx.Message}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while processing your request.",
                    details = ex.InnerException?.Message ?? ex.Message
                });
            }
        }


        //-------------NgocQuan----------------//
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("{orderId}/print-pdf-temp-Invoice")]
        public async Task<IActionResult> GetTempInvoicePdf(int orderId)
        {
            try
            {
                var pdfBytes = await _printOrderPdfService.GenerateTempInvoicePdfAsync(orderId);
                return File(pdfBytes, "application/pdf", $"Invoice_{orderId}.pdf");
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
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("get-customer-of-order/{orderId}")]
        public async Task<IActionResult> GetCustomerOfOrder([FromRoute] int orderId)
        {
            try
            {
                var customer = await _orderService.GetCustomerIdByOrderIdAsync(orderId); 

                return Ok(new
                {
                    success = true,
                    data = customer
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "A database update error occurred.",
                    details = ex.Message
                });
            }
            catch (DbException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "A general database error occurred.",
                    details = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }

        //Pham Son Tung
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpPost("assign-customer-to-order")]
        public async Task<IActionResult> AssignCustomerToOrder(
        [FromQuery] int orderId,
        [FromQuery] int customerId)
        {
            try
            {
                await _orderService.AssignCustomerToOrderAsync(orderId, customerId);
                await _hubContext.Clients.Group("order").SendAsync("CustomerOrderListUpdate", new
                {
                    customerId = customerId,
                });
                return Ok(new
                {
                    success = true,
                    message = $"Customer {customerId} has been assigned to order {orderId}."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "A database update error occurred.",
                    details = ex.Message
                });
            }
            catch (DbException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "A general database error occurred.",
                    details = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }

        //pham son tung
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpPost("remove-customer/{orderId}")]
        public async Task<IActionResult> RemoveCustomerFromOrder(int orderId)
        {
            var result = await _orderService.RemoveCustomerFromOrderAsync(orderId);
            if (result)
            {
                return Ok(new { Message = "Customer removed successfully from the order." });
            }
            else
            {
                return BadRequest(new { Message = "Failed to remove customer from the order." });
            }
        }

        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpPost("remove-order/{orderId}")]
        public async Task<IActionResult> RemoveOrder(int orderId)
        {
            try
            {
                var (orderDto, roomId, isUse) = await _orderService.RemoveOrderById(orderId);
                if (roomId.HasValue && isUse.HasValue)
                {
                    await _hubContext.Clients.Group("room").SendAsync("RoomStatusUpdate", new
                    {
                        roomId = roomId.Value,
                        isUse = isUse.Value
                    });
                }
                await _hubContext.Clients.Group("order").SendAsync("OrderListUpdate", new
                {
                    roomId = orderDto.RoomId,
                    shipperId = orderDto.ShipperId,
                    orderStatusId = orderDto.OrderStatusId
                });
                await _hubContext.Clients.Group("order").SendAsync("CustomerOrderListUpdate", new
                {
                    customerId = orderDto.CustomerId,
                });

                return Ok(new{Message = "Order has been successfully removed."});
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }


        //pham son tung
        [HttpGet("get-order-details-by-order-id")]
        public async Task<IActionResult> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            try
            {
                var orderDetails = await _orderService.GetOrderDetailsByOrderIdAsync(orderId);
                return Ok(orderDetails);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(new { message = $"Invalid parameter: {argEx.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        //Phạm Sơn Tùng
        [HttpPut("update-order-customer")]
        public async Task<IActionResult> UpdateOrderCustomer([FromBody] OrderUpdateDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.OrderId <= 0)
                return BadRequest("OrderId không hợp lệ.");
            try
            {
                var result = await _orderService.UpdateOrderWithDetailsAsync(request);

                if (!result)
                {
                    return StatusCode(500, "Cập nhật thất bại.");
                }
                await _hubContext.Clients.Group("order").SendAsync("CustomerUpdateOrder", request.OrderId);
                return Ok(new { message = "Cập nhật thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = "hehehe" + ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi cơ sở dữ liệu: " + ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }


        //Pham Son Tung
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("get-order-general-data/{orderId}")]
        public async Task<IActionResult> GetOrderGeneralDataPosAsync([FromRoute] int orderId)
        {
            try
            {
                var result = await _orderService.GetOrderGeneralDataPosAsync(orderId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = $"Order not found: {ex.Message}"
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    message = "A database query error occurred while retrieving order data.",
                    detail = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (InvalidOperationException invalidEx)
            {
                return StatusCode(500, new
                {
                    message = "An invalid operation occurred while retrieving order data.",
                    detail = invalidEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while processing the request.",
                    detail = ex.Message
                });
            }
        }

        //Pham Son Tung
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpDelete("delete-order-detail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteOrderDetail([FromQuery] string? orderId, [FromQuery] string? orderDetailId)
        {
            try
            {
                // Validate query parameters
                if (string.IsNullOrWhiteSpace(orderId) || !int.TryParse(orderId, out int orderIdParsed))
                {
                    return BadRequest(new { message = "Invalid 'orderId' parameter." });
                }

                if (string.IsNullOrWhiteSpace(orderDetailId) || !int.TryParse(orderDetailId, out int orderDetailIdParsed))
                {
                    return BadRequest(new { message = "Invalid 'orderDetailId' parameter." });
                }

                // Call service to delete
                await _orderService.DeleteOrderDetailAsync(orderIdParsed, orderDetailIdParsed);
                await _hubContext.Clients.Group("order").SendAsync("OrderUpdate", orderIdParsed);
                return Ok(new { message = "Order detail deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to delete order detail. The item may have already been processed or a database error occurred.",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while deleting the order detail.",
                    error = ex.Message
                });
            }
        }

        //Pham Son Tung
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpDelete("delete-confirmed-order-detail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteConfirmedOrderDetail(
            [FromQuery] int? orderId,
            [FromQuery] int? orderDetailId,
            [FromQuery] int? cashierId,
            [FromQuery] string? reason)
        {
            try
            {
                // Validate parameters
                if (orderId is null || orderId <= 0)
                    return BadRequest(new { message = "Parameter 'orderId' is required and must be a positive integer." });

                if (orderDetailId is null || orderDetailId <= 0)
                    return BadRequest(new { message = "Parameter 'orderDetailId' is required and must be a positive integer." });

                if (cashierId is null || cashierId <= 0)
                    return BadRequest(new { message = "Parameter 'cashierId' is required and must be a positive integer." });

                if (string.IsNullOrWhiteSpace(reason))
                    return BadRequest(new { message = "Parameter 'reason' is required and must not be empty." });
                var request = new DeleteConfirmedOrderDetailRequestDto
                {
                    OrderId = orderId.Value,
                    OrderDetailId = orderDetailId.Value,
                    CashierId = cashierId.Value,
                    Reason = reason.Trim()
                };
                await _orderService.DeleteConfirmedOrderDetailAsync(request);
                await _hubContext.Clients.Group("order").SendAsync("OrderUpdate", request.OrderId);
                return Ok(new { message = "Confirmed order detail was deleted and cancellation logged successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = "An operation error occurred while deleting the order detail.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        //Phạm Sơn Tùng
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("Get-order-payment-information/{orderId}")]
        public async Task<IActionResult> GetOrderPaymentInfo(string orderId)
        {
            try
            {
                // Kiểm tra input: null, rỗng, không phải số
                if (string.IsNullOrWhiteSpace(orderId))
                {
                    return BadRequest("Order ID must not be empty.");
                }

                if (!int.TryParse(orderId, out int parsedOrderId))
                {
                    return BadRequest("Order ID must be a valid integer.");
                }

                // Gọi service
                var orderDto = await _orderService.GetOrderPaymentDtoByIdAsync(parsedOrderId);
                if (orderDto == null)
                {
                    return NotFound($"Order with ID {parsedOrderId} not found.");
                }

                return Ok(orderDto);
            }
            catch (SqlException sqlEx)
            {
                // Lỗi kết nối SQL (truy xuất từ repo)
                return StatusCode(500, $"Database connection error: {sqlEx.Message}");
            }
            catch (DbException dbEx)
            {
                // Lỗi truy vấn CSDL (truy xuất từ repo)
                return StatusCode(500, $"Database query error: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                // Lỗi không xác định (trong service hoặc chung)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //Pham Son Tung
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpPost("add-order-delivery-information")]
        public async Task<IActionResult> AddOrderDeliveryInformationAsync([FromBody] DeliveryInformationCreateDto request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Request body không được null." });
            }
            if (request.OrderId <= 0)
            {
                return BadRequest(new { message = "OrderId không hợp lệ. Phải là số lớn hơn 0." });
            }

            if (string.IsNullOrWhiteSpace(request.ReceiverName))
            {
                return BadRequest(new { message = "ReceiverName không được để trống." });
            }

            if (request.ReceiverName.Length > 100)
            {
                return BadRequest(new { message = "ReceiverName không được vượt quá 100 ký tự." });
            }

            if (string.IsNullOrWhiteSpace(request.ReceiverPhone))
            {
                return BadRequest(new { message = "ReceiverPhone không được để trống." });
            }

            if (request.ReceiverPhone.Length > 20)
            {
                return BadRequest(new { message = "ReceiverPhone không được vượt quá 20 ký tự." });
            }

            if (string.IsNullOrWhiteSpace(request.ReceiverAddress))
            {
                return BadRequest(new { message = "ReceiverAddress không được để trống." });
            }

            if (request.ReceiverAddress.Length > 255)
            {
                return BadRequest(new { message = "ReceiverAddress không được vượt quá 255 ký tự." });
            }
            if (!string.IsNullOrEmpty(request.DeliveryNote) && request.DeliveryNote.Length > 255)
            {
                return BadRequest(new { message = "ReceiverAddress không được vượt quá 255 ký tự." });
            }

            try
            {
                var success = await _orderService.CreateDeliveryInformationServiceAsync(request);

                if (success)
                {
                    return Ok(new { message = "Tạo thông tin giao hàng thành công." });
                }
                else
                {
                    return StatusCode(500, new { message = "Tạo thông tin giao hàng thất bại." });
                }
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi khi thao tác với cơ sở dữ liệu.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi không xác định.", error = ex.Message });
            }
        }

        //Phạm Sơn Tùng
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("delivery-information/{orderId}")]
        public async Task<IActionResult> GetDeliveryInformationByOrderId(int orderId)
        {
            if (orderId <= 0)
            {
                return BadRequest(new { message = "OrderId không hợp lệ. Phải là số lớn hơn 0." });
            }

            try
            {
                var deliveryInfoDto = await _orderService.GetDeliveryInformationByOrderIdAsync(orderId);

                if (deliveryInfoDto == null)
                {
                    return NotFound(new { message = $"Không tìm thấy thông tin giao hàng cho order ID {orderId}." });
                }

                return Ok(new
                {
                    message = "Lấy thông tin giao hàng thành công.",
                    data = deliveryInfoDto
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { message = "Lỗi khi thao tác với cơ sở dữ liệu.", error = dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi không xác định.", error = ex.Message });
            }
        }

        //Phạm Sơn Tùng
        [HttpGet("order-ids-for-qr")]
        public async Task<IActionResult> GetOrderIdsForQrSite([FromQuery] string roomId, [FromQuery] string customerId)
        {
            try
            {
                // Kiểm tra null, rỗng, kiểu dữ liệu không hợp lệ
                if (string.IsNullOrWhiteSpace(roomId) || !int.TryParse(roomId, out int roomIdInt) || roomIdInt <= 0)
                {
                    return BadRequest("roomId không hợp lệ. Vui lòng cung cấp một số nguyên dương hợp lệ.");
                }

                if (string.IsNullOrWhiteSpace(customerId) || !int.TryParse(customerId, out int customerIdInt) || customerIdInt <= 0)
                {
                    return BadRequest("customerId không hợp lệ. Vui lòng cung cấp một số nguyên dương hợp lệ.");
                }

                // Gọi service
                var orderIds = await _orderService.GetOrderIdsForQrSiteAsync(roomIdInt, customerIdInt);

                return Ok(orderIds);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Lỗi cơ sở dữ liệu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        //Phạm Sơn Tùng
        [HttpPost("notify-customer-join")]
        public async Task<IActionResult> NotifyCustomerJoinAsync([FromBody] NotifyCustomerJoinDto request)
        {
            try
            {
                await _hubContext.Clients.Group("common").SendAsync("CustomerJoin", new
                {
                    roomId = request.RoomId,
                    customerId = request.CustomerId,
                    customerName = request.CustomerName,
                    phone = request.Phone
                });

                return Ok(new { message = "Đã gửi sự kiện CustomerJoin thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Gửi sự kiện thất bại.", error = ex.Message });
            }
        }
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("cancellation-history/{orderId}")]
        public async Task<IActionResult> GetOrderCancellationHistoryByIdAsync(int orderId)
        {
            try
            {
                var cancellations = await _orderService.GetOrderCancellationHistoryByIdAsync(orderId);
                if (cancellations == null || !cancellations.Any())
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Order cancellation history not found."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Order cancellation history retrieved successfully.",
                    data = cancellations
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Server error",
                    error = ex.Message
                });
            }
        }

        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("DeliveryInformation/{deliveryInformationId}")]
        public async Task<IActionResult> GetDeliveryInformationByIdAsync(int deliveryInformationId)
        {
            try
            {
                var DeliveryInformation = await _orderService.GetDeliveryInformationByIdAsync(deliveryInformationId);
                if (DeliveryInformation == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "DeliveryInformation not found."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "DeliveryInformation retrieved successfully.",
                    data = DeliveryInformation
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Server error",
                    error = ex.Message
                });
            }
        }

        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("OrderInformationById/{orderId}")]
        public async Task<IActionResult> GetOrderFullInforByIdAsync(int orderId)
        {
            try
            {
                var Order = await _orderService.GetOrderFullInforByIdAsync(orderId);
                if (Order == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Order not found."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Order retrieved successfully.",
                    data = Order
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Server error",
                    error = ex.Message
                });
            }
        }


        //Pham Son Tung
        [Authorize(Roles = "manager")]
        [HttpPost("filter-orders")]
        public async Task<IActionResult> FilterOrderManager([FromBody] FilterOrderManagerDto filterDto)
        {
            // Validate cơ bản
            if (filterDto.FromDate.HasValue && filterDto.ToDate.HasValue)
            {
                if (filterDto.FromDate > filterDto.ToDate)
                {
                    return BadRequest("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
                }
            }

            if (!string.IsNullOrWhiteSpace(filterDto.CustomerKeyword) && filterDto.CustomerKeyword.Length > 100)
            {
                return BadRequest("Từ khóa tìm kiếm khách hàng quá dài.");
            }

            if (filterDto.OrderId.HasValue && filterDto.OrderId <= 0)
            {
                return BadRequest("ID đơn hàng phải là số nguyên dương.");
            }

            try
            {
                var result = await _orderService.FilterOrderManagerAsync(filterDto);
                return Ok(new
                {
                    message = result.Message,
                    data = result.Data,
                    count = result.Data?.Count ?? 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Đã xảy ra lỗi khi lọc danh sách đơn hàng.",
                    Details = ex.Message
                });
            }
        }
    }
}