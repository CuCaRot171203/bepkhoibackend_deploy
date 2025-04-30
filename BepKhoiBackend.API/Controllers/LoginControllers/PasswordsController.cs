using BepKhoiBackend.BusinessObject.dtos.LoginDto;
using BepKhoiBackend.BusinessObject.Services;
using BepKhoiBackend.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BepKhoiBackend.BusinessObject.DTOs;
using BepKhoiBackend.BusinessObject.Services.LoginService.Interface;
using Org.BouncyCastle.Asn1.Ocsp;
using DocumentFormat.OpenXml.Office2016.Excel;

namespace BepKhoiBackend.API.Controllers.LoginControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordsController : Controller
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PasswordsController(IUserService userService, IEmailService emailService, IOtpService otpService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _emailService = emailService;
            _otpService = otpService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] EmailDto request)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (request == null || string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest(new { message = "Email is required." });
                }

                if (request.Email.Length > 255)
                {
                    return BadRequest(new { message = "Email must be at most 255 characters long." });
                }

                // Kiểm tra định dạng email hợp lệ
                if (!_userService.IsValidEmail(request.Email))
                {
                    return BadRequest(new { message = "Invalid email format." });
                }

                // Kiểm tra xem user có tồn tại trong database không
                var user = _userService.GetUserByEmail(request.Email);
                if (user == null)
                {
                    return BadRequest(new { message = "User with this email does not exist." });
                }

                // Tạo OTP
                var otp = _otpService.GenerateOtp(request.Email);

                // Gửi OTP qua email
                await _emailService.SendEmailAsync(request.Email, "Your OTP Code", $"Your OTP is: {otp}");

                return Ok(new { message = "OTP sent successfully." });
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





        [HttpPost("verify")]
        public async Task<IActionResult> VerifyUser([FromBody] VerifyOtpDto request)
        {
            try
            {
                // Kiểm tra request có bị null không
                if (request == null)
                {
                    return BadRequest(new { message = "Invalid request data." });
                }

                // Kiểm tra email và OTP không được để trống
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
                {
                    return BadRequest(new { message = "Email and OTP are required." });
                }

                // Kiểm tra độ dài email không vượt quá 255 ký tự
                if (request.Email.Length > 255)
                {
                    return BadRequest(new { message = "Email must be at most 255 characters long." });
                }

                // Kiểm tra email có đúng định dạng không
                if (!_userService.IsValidEmail(request.Email))
                {
                    return BadRequest(new { message = "Invalid email format." });
                }

                // Kiểm tra OTP có đúng 6 chữ số không (chỉ chứa số)
                if (!System.Text.RegularExpressions.Regex.IsMatch(request.Otp, @"^\d{6}$"))
                {
                    return BadRequest(new { message = "OTP must be a 6-digit numeric code." });
                }

                // Xác thực người dùng với OTP
                bool isVerified = await _userService.VerifyUserByEmail(request.Email, request.Otp);
                if (!isVerified)
                {
                    return BadRequest(new { message = "Invalid OTP or Email." });
                }

                // Lấy thông tin user từ database
                var user = _userService.GetUserByEmail(request.Email);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                // Tạo token cho user
                var token = _userService.GenerateJwtToken(user);

                // Lưu thông tin vào Session
                var session = _httpContextAccessor.HttpContext.Session;
                session.SetString("Token", token);
                session.SetString("UserId", user.UserId.ToString());
                session.SetString("Phone", user.Email);

                return Ok(new { message = "Verification successful!", token, userId = user.UserId, RoleName = user.RoleName, UserName = user.UserName });
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


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] String email)
        {
            try
            {

                // Kiểm tra email và mật khẩu không được để trống
                if (string.IsNullOrWhiteSpace(email) )
                {
                    return BadRequest(new { message = "Email are required." });
                }

                // Kiểm tra độ dài email không vượt quá 255 ký tự
                if (email.Length > 255)
                {
                    return BadRequest(new { message = "Email must be at most 255 characters long." });
                }

                // Kiểm tra email có đúng định dạng không
                if (!_userService.IsValidEmail(email))
                {
                    return BadRequest(new { message = "Invalid email format." });
                }

                //
                if(_userService.GetUserByEmail ==  null)
                {
                    return BadRequest(new { message = "user not exist" });

                }
                var password = _userService.GenerateRandomPassword();
                // Gọi service để thực hiện đặt lại mật khẩu
                var result = await _userService.ForgotPassword(email,password);
                //sent pass to email
                await _emailService.SendEmailAsync(email, "", $"Your Password is: {password}");
                if (!result)
                {
                    return NotFound(new { message = "Email not found!" });
                }

                return Ok(new { message = "Reset successful!" });
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


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            try
            {
                // Kiểm tra request có bị null không
                if (request == null)
                {
                    return BadRequest(new { message = "Invalid request data." });
                }

                // Kiểm tra tất cả các trường không được để trống
                if (string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.OldPassword) ||
                    string.IsNullOrWhiteSpace(request.NewPassword) ||
                    string.IsNullOrWhiteSpace(request.RePassword))
                {
                    return BadRequest(new { message = "All fields are required!" });
                }

                // Kiểm tra độ dài email không vượt quá 255 ký tự
                if (request.Email.Length > 255)
                {
                    return BadRequest(new { message = "Email must be at most 255 characters long." });
                }

                // Kiểm tra email có đúng định dạng không
                if (!_userService.IsValidEmail(request.Email))
                {
                    return BadRequest(new { message = "Invalid email format." });
                }

                // Kiểm tra độ dài mật khẩu không vượt quá 255 ký tự
                if (request.NewPassword.Length > 255 || request.OldPassword.Length > 255 || request.RePassword.Length > 255)
                {
                    return BadRequest(new { message = "Passwords must be at most 255 characters long." });
                }

                // Kiểm tra mật khẩu mới và nhập lại mật khẩu có trùng nhau không
                if (request.NewPassword != request.RePassword)
                {
                    return BadRequest(new { message = "New passwords do not match!" });
                }

                // Gọi service để thay đổi mật khẩu
                var result = await _userService.ChangePassword(request);

                if (result == "UserNotFound")
                {
                    return NotFound(new { message = "Email not found!" });
                }

                if (result == "WrongPassword")
                {
                    return Unauthorized(new { message = "Old password is incorrect!" });
                }

                return Ok(new { message = "Password changed successfully!" });
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
