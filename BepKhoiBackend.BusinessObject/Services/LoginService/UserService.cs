using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BepKhoiBackend.BusinessObject.dtos.LoginDto;
using BepKhoiBackend.BusinessObject.DTOs;
using BepKhoiBackend.BusinessObject.Services.LoginService.Interface;
using BepKhoiBackend.DataAccess.Repository.LoginRepository.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpService _otpService;
    private readonly IConfiguration _configuration;
    private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";

    public UserService(IUserRepository userRepository, IOtpService otpService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _otpService = otpService;
        _configuration = configuration;
    }

    public UserDto? GetUserByEmail(string email)
    {
        var user = _userRepository.GetUserByEmail(email);
        if (user == null)
        {
            return null; // Trả về null nếu không tìm thấy user
        }

        return new UserDto
        {
            UserId = user.UserId,
            Email = user.Email,
            IsVerify = user.IsVerify,
            UserName = user.UserInformation.UserName,
            RoleName = user.Role.RoleName
        };
    }


    public string GenerateRandomPassword()
    {
        int length = 16;
        var password = new StringBuilder();
        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[sizeof(uint)];
            for (int i = 0; i < length; i++)
            {
                rng.GetBytes(bytes);
                uint randomNumber = BitConverter.ToUInt32(bytes, 0);
                var index = randomNumber % Chars.Length;
                password.Append(Chars[(int)index]);
            }
        }
        return password.ToString();
    }

    public async Task<bool> VerifyUserByEmail(string email, string otp)
    {
        if (!_otpService.VerifyOtp(email, otp))
            return false;

        var user = _userRepository.GetUserByEmail(email);
        if (user == null)
            return false;
        if (user.Status != true || user.IsDelete == true)
        {
            throw new UnauthorizedAccessException();

        }

        user.IsVerify = true;
        _userRepository.UpdateUser(user);
        return true;
    }

    public string GenerateJwtToken(UserDto user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        string secretKey = _configuration["Jwt:Key"];

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new ArgumentNullException("Jwt:Key", "JWT secret key is missing in configuration.");
        }

        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.RoleName)
        }),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    //code logic reset password
    public async Task<bool> ForgotPassword(string email, string password)
    {
        var user = _userRepository.GetUserByEmail(email);
        if (user == null)
        {
            return false; // Người dùng không tồn tại
        }
        // Mã hóa mật khẩu mới
        user.Password = password;
        _userRepository.UpdateUser(user);
        return true;
    }
    // Hàm kiểm tra định dạng email hợp lệ
    public bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    //change password
    public async Task<string> ChangePassword(ChangePasswordDto request)
    {
        var user = _userRepository.GetUserByEmail(request.Email);
        if (user == null)
        {
            return "UserNotFound"; // Không tìm thấy tài khoản
        }

        // Kiểm tra mật khẩu cũ có đúng không (không dùng BCrypt)
        if (user.Password != request.OldPassword)
        {
            return "WrongPassword"; // Sai mật khẩu cũ
        }

        // Mã hóa mật khẩu mới và cập nhật vào database
        //user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.Password = request.NewPassword;
        _userRepository.UpdateUser(user);

        return "Success"; // Đổi mật khẩu thành công
    }


    public async Task<ResponseUserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null) return null;

        return new ResponseUserDto
        {
            UserId = user.UserId,
            UserName = user.UserInformation?.UserName,
            RoleName = user.Role?.RoleName,
            Phone = user.Phone,
            Email = user.Email,
            Address = user.UserInformation?.Address,
            Province_City = user.UserInformation?.ProvinceCity,
            District = user.UserInformation?.District,
            Ward_Commune = user.UserInformation?.WardCommune,
            Date_of_Birth = user.UserInformation?.DateOfBirth
        };
    }

    //Phạm Sơn Tùng
    public async Task UpdateUserStatusAsync(int userId, bool status)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than zero.");
        }

        await _userRepository.UpdateUserStatusAsync(userId, status);
    }

    public async Task DeleteUserAsync(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than zero.");
        }

        await _userRepository.UpdateUserIsDeleteAsync(userId);
    }
}
