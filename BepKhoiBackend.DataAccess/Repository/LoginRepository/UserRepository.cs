using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BepKhoiBackend.DataAccess.Repository.LoginRepository.Interface;

namespace BepKhoiBackend.DataAccess.Repository.LoginRepository
{ 
public class UserRepository : IUserRepository
{
    private readonly bepkhoiContext _context;

    public UserRepository(bepkhoiContext context)
    {
        _context = context;
    }

        public User? GetUserByEmail(string email)
        {
            return _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.UserInformation)
                    .FirstOrDefault(u => u.Email == email);
        }

        public void UpdateUser(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }
    public async Task<User?> GetUserByIdAsync(int userId)
    {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.UserInformation)
                .FirstOrDefaultAsync(u => u.UserId == userId && (u.IsDelete == null || u.IsDelete == false));
    }


        //Phạm Sơn Tùng
        public async Task UpdateUserStatusAsync(int userId, bool status)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    throw new ArgumentException($"User with ID {userId} not found.");
                }
                user.Status = status;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task UpdateUserIsDeleteAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    throw new ArgumentException($"User with ID {userId} not found.");
                }
                user.IsDelete = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }






    }
}