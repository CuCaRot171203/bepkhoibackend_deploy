using BepKhoiBackend.BusinessObject.dtos.UserDto.CashierDto;
using BepKhoiBackend.BusinessObject.Services.UserService.CashierService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BepKhoiBackend.API.Controllers.UserControllers.CashierControllers
{
    [Authorize]
    [Route("api/cashiers")]
    [ApiController]
    public class CashierController : ControllerBase
    {
        private readonly ICashierService _cashierService;

        public CashierController(ICashierService cashierService)
        {
            _cashierService = cashierService;
        }

        [Authorize(Roles = "manager")]
        // Lấy danh sách tất cả Cashier
        [HttpGet]
        public ActionResult<IEnumerable<CashierDTO>> GetAllCashiers()
        {
            var cashiers = _cashierService.GetAllCashiers();
            return Ok(cashiers);
        }

        [Authorize(Roles = "manager")]
        // Lấy thông tin Cashier theo ID
        [HttpGet("{id}")]
        public ActionResult<CashierDTO> GetCashierById(int id)
        {
            var cashier = _cashierService.GetCashierById(id);
            if (cashier == null)
            {
                return NotFound(new { message = "Cashier not found" });
            }
            return Ok(cashier);
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        //[Authorize(Roles = "Admin, Manager")]
        public IActionResult CreateCashier([FromBody] CreateCashierDTO newCashier)
        {
            if (newCashier == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            _cashierService.CreateCashier(newCashier.Email, newCashier.Password, newCashier.Phone, newCashier.UserName);
            return Ok("Cashier đã được tạo thành công.");
        }



        [HttpPut("{id}")]
        [Authorize(Roles = "manager, cashier")]
        public async Task<IActionResult> UpdateCashier(int id, [FromBody] UpdateCashierDTO updatedCashier)
        {
            try
            {
                if (updatedCashier == null)
                {
                    return BadRequest("Dữ liệu không hợp lệ.");
                }

                var isUpdated = await _cashierService.UpdateCashier(
                    id,
                    updatedCashier.Email,
                    updatedCashier.Phone,
                    updatedCashier.UserName,
                    updatedCashier.Address,
                    updatedCashier.ProvinceCity,
                    updatedCashier.District,
                    updatedCashier.WardCommune,
                    updatedCashier.DateOfBirth
                );

                if (!isUpdated)
                {
                    return BadRequest("Cập nhật cashier thất bại. Kiểm tra lại thông tin.");
                }

                return Ok($"Cashier có ID {id} đã được cập nhật thành công.");
            }
            catch (InvalidOperationException)
            {
                return BadRequest("Cập nhật cashier thất bại. Kiểm tra lại thông tin.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi khi cập nhật Cashier: {ex.Message}");
            }
        }

        [Authorize(Roles = "manager")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCashier(int id)
        {
            _cashierService.DeleteCashier(id);
            return Ok($"Cashier có ID {id} đã bị xóa.");
        }

        [Authorize(Roles = "manager")]
        [HttpGet("{id}/invoices")]
        public ActionResult<IEnumerable<CashierInvoiceDTO>> GetCashierInvoices(int id)
        {
            var invoices = _cashierService.GetCashierInvoices(id);
            if (invoices == null || !invoices.Any())
            {
                return NotFound($"Không có hóa đơn nào cho cashier với ID {id}.");
            }
            return Ok(new { CashierId = id, Invoices = invoices });
        }

        [Authorize(Roles = "manager")]
        [HttpGet("search")]
        public ActionResult<List<CashierDTO>> GetCashiers([FromQuery] string? searchTerm, [FromQuery] bool? status)
        {
            var cashiers = _cashierService.GetCashiers(searchTerm, status);
            return Ok(cashiers);
        }

        [Authorize(Roles = "manager")]
        // Xuất danh sách Cashiers ra Excel
        [HttpGet("export")]
        public IActionResult ExportCashiersToExcel()
        {
            var fileContent = _cashierService.ExportCashiersToExcel();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Cashiers.xlsx");
        }
    }
}
