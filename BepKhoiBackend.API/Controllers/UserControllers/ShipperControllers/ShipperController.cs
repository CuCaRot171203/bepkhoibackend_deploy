using BepKhoiBackend.BusinessObject.dtos.UserDto.ShipperDto;
using BepKhoiBackend.BusinessObject.Services.UserService.ShipperService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BepKhoiBackend.API.Controllers.UserControllers.ShipperControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipperController : ControllerBase
    {
        private readonly IShipperService _shipperService;

        public ShipperController(IShipperService shipperService)
        {
            _shipperService = shipperService;
        }

        [HttpGet]
        [Authorize(Roles = "manager, cashier, shipper")]
        public ActionResult<IEnumerable<ShipperDTO>> GetAllShippers()
        {
            try
            {
                var shippers = _shipperService.GetAllShippers();
                if (shippers == null || shippers.Count == 0)
                {
                    return NotFound("Không có shipper nào trong hệ thống.");
                }
                return Ok(shippers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "manager, cashier")]
        public ActionResult<ShipperDTO> GetShipperById(int id)
        {
            try
            {
                var shipper = _shipperService.GetShipperById(id);
                if (shipper == null)
                {
                    return NotFound($"Không tìm thấy shipper có ID: {id}");
                }
                return Ok(shipper);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public IActionResult CreateShipper([FromBody] CreateShipperDTO newShipper)
        {
            try
            {
                if (newShipper == null)
                {
                    return BadRequest("Dữ liệu không hợp lệ.");
                }

                _shipperService.CreateShipper(newShipper.Email, newShipper.Password, newShipper.Phone, newShipper.UserName);
                return Ok("Shipper đã được tạo thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "manager, shipper")]
        public IActionResult UpdateShipper(int id, [FromBody] UpdateShipperDTO updatedShipper)
        {
            try
            {
                if (updatedShipper == null)
                {
                    return BadRequest("Dữ liệu không hợp lệ.");
                }

                var isUpdated = _shipperService.UpdateShipper(
                    id,
                    updatedShipper.Email,
                    updatedShipper.Phone,
                    updatedShipper.UserName,
                    updatedShipper.Address,
                    updatedShipper.ProvinceCity,
                    updatedShipper.District,
                    updatedShipper.WardCommune,
                    updatedShipper.DateOfBirth
                );

                if (!isUpdated)
                {
                    return BadRequest("Cập nhật shipper thất bại. Kiểm tra lại thông tin.");
                }

                return Ok($"Shipper có ID {id} đã được cập nhật thành công.");
            }
            catch (InvalidOperationException)
            {
                return BadRequest("Cập nhật shipper thất bại. Kiểm tra lại thông tin.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi khi cập nhật Shipper: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "manager")]
        public IActionResult DeleteShipper(int id)
        {
            try
            {
                _shipperService.DeleteShipper(id);
                return Ok($"Shipper có ID {id} đã bị xóa.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }


        [HttpGet("{id}/invoices")]
        [Authorize(Roles = "manager")]
        public ActionResult<IEnumerable<ShipperInvoiceDTO>> GetShipperInvoices(int id)
        {
            try
            {
                var invoices = _shipperService.GetShipperInvoices(id);
                if (invoices == null || !invoices.Any())
                {
                    return NotFound($"Không có hóa đơn nào cho shipper với ID {id}.");
                }
                return Ok(new { ShipperId = id, Invoices = invoices });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }


        [HttpGet("search")]
        [Authorize(Roles = "manager")]
        public ActionResult<List<ShipperDTO>> GetShippers([FromQuery] string? searchTerm, [FromQuery] bool? status)
        {
            try
            {
                var shippers = _shipperService.GetShippers(searchTerm, status);
                return shippers is { Count: > 0 } ? Ok(shippers) : NotFound("Không tìm thấy shipper nào.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }


        [HttpGet("export")]
        [Authorize(Roles = "manager")]
        public IActionResult ExportShippersToExcel()
        {
            try
            {
                var fileContents = _shipperService.ExportShippersToExcel();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Shippers.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
