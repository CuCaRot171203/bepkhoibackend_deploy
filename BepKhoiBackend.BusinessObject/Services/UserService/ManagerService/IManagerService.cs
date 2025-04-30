using BepKhoiBackend.BusinessObject.dtos.UserDto.ManagerDto;
using System;

namespace BepKhoiBackend.BusinessObject.Services.UserService.ManagerService
{
    public interface IManagerService
    {
        GetManagerDTO? GetManagerById(int id);
        Task<bool> UpdateManagerAsync(int userId, string email, string phone, string? userName, string? address,
                                                   string? provinceCity, string? district, string? wardCommune, DateTime? dateOfBirth);
    }
}
