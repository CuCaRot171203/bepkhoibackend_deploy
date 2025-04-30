using BepKhoiBackend.BusinessObject.dtos.LoginDto;
using BepKhoiBackend.BusinessObject.DTOs;

namespace BepKhoiBackend.BusinessObject.Services.LoginService.Interface
{
    public interface IAuthService
    {
        UserDto? ValidateUser(LoginRequestDto loginRequest);
        string GenerateJwtToken(UserDto user);

        bool IsValidEmail(string email);

    }
}
