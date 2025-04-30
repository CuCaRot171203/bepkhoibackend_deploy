using BepKhoiBackend.BusinessObject.dtos.LoginDto;
using BepKhoiBackend.BusinessObject.DTOs;
using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.BusinessObject.Services.LoginService.Interface
{

    public interface IUserService
    {
        public UserDto? GetUserByEmail(string email);
        Task<bool> VerifyUserByEmail(string email, string otp);
        string GenerateJwtToken(UserDto user);
        Task<bool> ForgotPassword(string email, string password);
        Task<string> ChangePassword(ChangePasswordDto request);
        bool IsValidEmail(string email);
        Task<ResponseUserDto?> GetUserByIdAsync(int userId);
        public string GenerateRandomPassword();
        Task UpdateUserStatusAsync(int userId, bool status);
        Task DeleteUserAsync(int userId);

    }

}
