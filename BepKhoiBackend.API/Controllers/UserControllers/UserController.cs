using BepKhoiBackend.BusinessObject.Services.LoginService.Interface;
using BepKhoiBackend.BusinessObject.Services.UserService.ShipperService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BepKhoiBackend.API.Controllers.UserControllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "manager, cashier, shipper")]
        [HttpGet("get-user-by-id/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var userDto = await _userService.GetUserByIdAsync(userId);

            if (userDto == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                message = "User fetched successfully",
                data = userDto
            });
        }

        //Phạm Sơn Tùng
        [Authorize(Roles = "manager")]
        [HttpPut("status/{userId}")]
        public async Task<IActionResult> UpdateUserStatus(int userId, [FromQuery] bool status)
        {
            try
            {
                await _userService.UpdateUserStatusAsync(userId, status);
                return Ok(new { message = "User status updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "A database error occurred while updating user status.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Roles = "manager")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                await _userService.DeleteUserAsync(userId);
                return Ok(new { message = "User deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "A database error occurred while deleting the user.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }



    }
}
