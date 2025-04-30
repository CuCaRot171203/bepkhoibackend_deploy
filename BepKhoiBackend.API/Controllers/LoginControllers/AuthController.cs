using Microsoft.AspNetCore.Mvc;
using BepKhoiBackend.BusinessObject.DTOs;
using BepKhoiBackend.BusinessObject.dtos.LoginDto;
using Microsoft.AspNetCore.Http;
using System;
using DocumentFormat.OpenXml.Office2016.Excel;
using BepKhoiBackend.BusinessObject.Services.LoginService.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using CloudinaryDotNet.Actions;

namespace BepKhoiBackend.API.Controllers.LoginControllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(IAuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                // Xoá dữ liệu trong session
                var session = _httpContextAccessor.HttpContext.Session;
                session.Remove("Token");
                session.Remove("UserId");
                session.Remove("Phone");

                // Hoặc xóa toàn bộ session
                session.Clear();

                return Ok(new { message = "Logout successful." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while logging out.",
                    error = ex.Message
                });
            }
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (loginRequest == null)
                {
                    return BadRequest(new { message = "Invalid request data." });
                }

                if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
                {
                    return BadRequest(new { message = "Email and Password are required." });
                }

                if (loginRequest.Email.Length > 255)
                {
                    return BadRequest(new { message = "Email must be at most 255 characters long." });
                }
                // Kiểm tra email có đúng định dạng không
                if (!_authService.IsValidEmail(loginRequest.Email))
                {
                    return BadRequest(new { message = "Invalid email format." });
                }
                if (loginRequest.Password.Length > 255)
                {
                    return BadRequest(new { message = "Password must be at most 255 characters long." });
                }

                var user = _authService.ValidateUser(loginRequest);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email, password. " });
                }

                if (!user.IsVerify == true)
                {
                    return Ok(new { message = "not_verify" });
                }

                var token = _authService.GenerateJwtToken(user);
                // Lưu thông tin vào Session
                var session = _httpContextAccessor.HttpContext.Session;
                session.SetString("Token", token);
                session.SetString("UserId", user.UserId.ToString());
                session.SetString("Email", user.Email);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                return Ok(new { message = "successful", token, userId = user.UserId, RoleName = user.RoleName, UserName = user.UserName });
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "deactived account."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while processing your request.",
                    error = ex.Message
                });
            }
        }
    }
}
