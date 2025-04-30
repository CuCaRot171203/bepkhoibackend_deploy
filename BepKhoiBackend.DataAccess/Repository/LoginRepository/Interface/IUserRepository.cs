using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.DataAccess.Repository.LoginRepository.Interface
{ 

    public interface IUserRepository
    {
        User? GetUserByEmail(string email);
        void UpdateUser(User user);
        Task<User?> GetUserByIdAsync(int userId);
        Task UpdateUserStatusAsync(int userId, bool status);
        Task UpdateUserIsDeleteAsync(int userId);
    }
}