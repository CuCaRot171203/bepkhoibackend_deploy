using BepKhoiBackend.BusinessObject.Services.UserService.ManagerService;
using BepKhoiBackend.BusinessObject.dtos.UserDto.ManagerDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BepKhoiBackend.API.Controllers.UserControllers.ManagerControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        // Lấy thông tin Manager theo ID
        [Authorize(Roles = "manager")]
        [HttpGet("{id}")]
        public IActionResult GetManagerById(int id)
        {
            try
            {
                var manager = _managerService.GetManagerById(id);
                if (manager == null)
                {
                    return NotFound($"Không tìm thấy Manager với ID {id}.");
                }
                return Ok(manager);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi khi lấy thông tin Manager: {ex.Message}");
            }
        }

        // Cập nhật thông tin Manager
        [Authorize(Roles = "manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateManager(int id, [FromBody] UpdateManagerDTO updatedManager)
        {
            if (updatedManager == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            // Validate cơ bản
            if (string.IsNullOrWhiteSpace(updatedManager.Email))
            {
                return BadRequest("Email không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(updatedManager.Phone))
            {
                return BadRequest("Số điện thoại không được để trống.");
            }

            try
            {
                var isUpdated = await _managerService.UpdateManagerAsync(
                    id,
                    updatedManager.Email,
                    updatedManager.Phone,
                    updatedManager.UserName,
                    updatedManager.Address,
                    updatedManager.ProvinceCity,
                    updatedManager.District,
                    updatedManager.WardCommune,
                    updatedManager.DateOfBirth
                );

                if (!isUpdated)
                {
                    return BadRequest("Cập nhật Manager thất bại. Kiểm tra lại thông tin.");
                }

                return Ok($"Manager có ID {id} đã được cập nhật thành công.");
            }
            catch (InvalidOperationException)
            {
                return BadRequest("Cập nhật Manager thất bại. Kiểm tra lại thông tin.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi khi cập nhật Manager: {ex.Message}");
            }
        }



    }
}
