using BepKhoiBackend.API.Hubs;
using BepKhoiBackend.BusinessObject.dtos.InvoiceDto;
using BepKhoiBackend.BusinessObject.Services.InvoiceService;
using BepKhoiBackend.DataAccess.Models.ExtendObjects;
using BepKhoiBackend.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BepKhoiBackend.API.Controllers.InvoiceControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IHubContext<SignalrHub> _hubContext;
        private readonly IInvoiceService _invoiceService;
        private readonly VnPayService _vnPayService;
        private readonly PrintInvoicePdfService _pdfService;
        public InvoiceController(IInvoiceService invoiceService, VnPayService vnPayService, PrintInvoicePdfService pdfService, IHubContext<SignalrHub> hubContext)
        {
            _invoiceService = invoiceService;
            _vnPayService = vnPayService;
            _pdfService = pdfService;
            _hubContext = hubContext;
        }

        [Authorize]
        [Authorize(Roles = "manager")]
        [HttpGet]
        public async Task<ActionResult<List<InvoiceDTO>>> GetAllInvoices()
        {
            try
            {
                var result = await _invoiceService.GetAllInvoicesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Đã xảy ra lỗi khi lấy danh sách hóa đơn.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "manager")]
        [HttpPost("filter-invoices")]
        public async Task<IActionResult> FilterInvoices([FromBody] FilterInvoiceManager filterDto)
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

            if (!string.IsNullOrWhiteSpace(filterDto.CashierKeyword) && filterDto.CashierKeyword.Length > 100)
            {
                return BadRequest("Từ khóa tìm kiếm thu ngân quá dài.");
            }

            try
            {
                var result = await _invoiceService.FilterInvoiceManagerServiceAsync(filterDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Đã xảy ra lỗi khi lọc danh sách hóa đơn.",
                    Details = ex.Message
                });
            }
        }
        //------------------NgocQuan----------------------//
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("{id}/print-pdf")]
        public IActionResult GetInvoicePdf(int id)
        {
            var invoice = _invoiceService.GetInvoiceForPdf(id);

            if (invoice == null)
            {
                return NotFound($"Không tìm thấy hóa đơn với ID {id}");
            }

            var pdfBytes = _pdfService.GenerateInvoicePdf(invoice);
            return File(pdfBytes, "application/pdf", $"Invoice_{id}.pdf");
        }


        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpGet("vnpay-url")]
        public async Task<IActionResult> CreatePaymentUrlVnpay([FromQuery] int Id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdForVnpayAsync(Id);
            if (invoice == null)
            {
                return NotFound($"Không tìm thấy hóa đơn với ID {Id}");
            }

            if (invoice.AmountDue <= 0)
            {
                return BadRequest("Số tiền thanh toán không hợp lệ.");
            }

            var model = new PaymentInformationModel
            {
                OrderType = "other",
                Amount = (int)invoice.AmountDue,
                InvoiceId = invoice.InvoiceId.ToString(),
                Name = invoice.CustomerName ?? "Khách Lẻ"
            };

            try
            {
                var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
                return Ok(url);
            }
            catch (Exception)
            {
                return StatusCode(500, "Có lỗi xảy ra trong quá trình tạo URL thanh toán.");
            }
        }

        [HttpGet("Return")]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.Success && response.VnPayResponseCode == "00")
            {
                if (int.TryParse(response.InvoiceId, out int invoiceId))
                {
                    await _invoiceService.UpdateInvoiceStatus(invoiceId, true);
                    //Gửi sự kiện thanh toán thành công
                    await _hubContext.Clients.Group("payment").SendAsync("PaymentStatus", new
                    {
                        invoiceId = response.InvoiceId,
                        status = true
                    });
                    //Cập nhật lại trạng thái order và iseUse của room
                    var (invoice, roomUpdateResult) = await _invoiceService.HandleInvoiceVnpayCompletionAsync(invoiceId);
                    //Gửi sự kiện cập nhật danh sách order 
                    await _hubContext.Clients.Group("order").SendAsync("OrderListUpdate", new
                    {
                        roomId = invoice.RoomId,
                        shipperId = invoice.ShipperId,
                        orderStatusId = invoice.OrderTypeId
                    });
                    // Gửi sự kiện RoomStatusUpdate nếu có cập nhật trạng thái phòng
                    if (roomUpdateResult?.roomId != null && roomUpdateResult?.isUse != null)
                    {
                        await _hubContext.Clients.Group("room").SendAsync("RoomStatusUpdate", new
                        {
                            roomId = roomUpdateResult.Value.roomId!.Value,
                            isUse = roomUpdateResult.Value.isUse!.Value
                        });
                    }
                    // Gửi sự kiện CustomerOrderListUpdate nếu có thông tin khách hàng
                    if (invoice.RoomId.HasValue && invoice.CustomerId.HasValue && invoice.Status == true)
                    {
                        await _hubContext.Clients.Group("order").SendAsync("CustomerOrderListUpdate", new
                        {
                            customerId = invoice.CustomerId
                        });
                    }
                    // Redirect đến frontend (ví dụ: trang thanh toán thành công)
                    var redirectUrl = $"http://localhost:3000/vnpay-result?result=true";
                    return Redirect(redirectUrl);
                }
            else
                {
                    await _hubContext.Clients.Group("payment").SendAsync("PaymentStatus", new
                    {
                        invoiceId = response.InvoiceId,
                        status = false
                    });
                    var failUrl = $"http://localhost:3000/vnpay-result?result=false";
                    return Redirect(failUrl);
                }
            }

            // Redirect đến trang thất bại
            await _hubContext.Clients.Group("payment").SendAsync("PaymentStatus", new
            {
                invoiceId = response.InvoiceId,
                status = false
            });
            var redirectFail = $"http://localhost:3000/vnpay-result?result=false";
            return Redirect(redirectFail);
        }



        //Phạm Sơn Tùng
        public class InvoicePaymentRequestDto
        {
            public InvoiceForPaymentDto InvoiceInfo { get; set; } = null!;
            public List<InvoiceDetailForPaymentDto> InvoiceDetails { get; set; } = new();
        }
        [Authorize]
        [Authorize(Roles = "manager, cashier")]
        [HttpPost("create-invoice-for-payment")]
        public async Task<IActionResult> CreateInvoiceForPaymentAsync([FromBody] InvoicePaymentRequestDto request)
        {
            try
            {
                if (request == null || request.InvoiceInfo == null || request.InvoiceDetails == null || !request.InvoiceDetails.Any())
                {
                    return BadRequest(new { message = "Dữ liệu hóa đơn không hợp lệ." });
                }
                var invoice = request.InvoiceInfo;
                // Kiểm tra các điều kiện hợp lệ
                if (invoice.PaymentMethodId != 1 && invoice.PaymentMethodId != 2)
                    return BadRequest(new { message = "Phương thức thanh toán không hợp lệ. Chỉ chấp nhận 1 hoặc 2." });

                if (invoice.OrderTypeId < 1 || invoice.OrderTypeId > 3)
                    return BadRequest(new { message = "Loại đơn hàng không hợp lệ. Chỉ chấp nhận 1, 2 hoặc 3." });

                if (invoice.OrderId <= 0)
                    return BadRequest(new { message = "Mã đơn hàng không hợp lệ." });

                if (invoice.CashierId <= 0)
                    return BadRequest(new { message = "Mã thu ngân không hợp lệ." });

                if (invoice.CustomerId.HasValue && invoice.CustomerId <= 0)
                    return BadRequest(new { message = "Mã khách hàng không hợp lệ." });

                if (invoice.RoomId.HasValue && invoice.RoomId <= 0)
                    return BadRequest(new { message = "Mã phòng không hợp lệ." });

                if (invoice.ShipperId.HasValue && invoice.ShipperId <= 0)
                    return BadRequest(new { message = "Mã shipper không hợp lệ." });

                if (invoice.CheckInTime == default || invoice.CheckOutTime == default)
                    return BadRequest(new { message = "Thời gian check-in hoặc check-out không hợp lệ." });

                if (invoice.TotalQuantity <= 0)
                    return BadRequest(new { message = "Tổng số lượng sản phẩm phải lớn hơn 0." });

                foreach (var detail in request.InvoiceDetails)
                {
                    if (detail.ProductId <= 0)
                        return BadRequest(new { message = "Mã sản phẩm không hợp lệ." });

                    if (string.IsNullOrWhiteSpace(detail.ProductName))
                        return BadRequest(new { message = "Tên sản phẩm không được để trống." });

                    if (detail.Quantity <= 0)
                        return BadRequest(new { message = $"Số lượng của sản phẩm '{detail.ProductName}' phải lớn hơn 0." });
                }
                var (invoiceId, roomId, isUse) = await _invoiceService.CreateInvoiceForPaymentServiceAsync(
                    request.InvoiceInfo,
                    request.InvoiceDetails
                );
                if (invoice.Status == true)
                {
                    await _hubContext.Clients.Group("order").SendAsync("OrderListUpdate", new
                    {
                        roomId = invoice.RoomId,
                        shipperId = invoice.ShipperId,
                        orderStatusId = invoice.OrderTypeId
                    });
                }
                if (roomId.HasValue && isUse.HasValue)
                {
                    await _hubContext.Clients.Group("room").SendAsync("RoomStatusUpdate", new
                    {
                        roomId = roomId.Value,
                        isUse = isUse.Value
                    });
                }
                if (invoice.RoomId.HasValue && invoice.CustomerId.HasValue && invoice.Status == true)
                {
                    await _hubContext.Clients.Group("order").SendAsync("CustomerOrderListUpdate", new
                    {
                        customerId = invoice.CustomerId,
                    });
                }
                return Ok(new
                {
                    message = "Tạo hóa đơn thanh toán thành công.",
                    invoiceId = invoiceId
                });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi CSDL khi tạo hóa đơn.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi không xác định.", error = ex.Message });
            }
        }





    }
}