using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.UserRepository.ManagerRepository
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly bepkhoiContext _context;

        public ManagerRepository(bepkhoiContext context)
        {
            _context = context;
        }

        // Lấy thông tin Manager theo ID
        public User? GetManagerById(int id)
        {
            try
            {
                return _context.Users
                    .Include(u => u.UserInformation)
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.UserId == id && u.Role.RoleName == "manager" && u.IsDelete != true && u.Status == true);
            }catch(Exception)
            {
                throw;
            }
        }

        // Cập nhật thông tin Manager
        public async Task<bool> UpdateManagerAsync(int userId, string email, string phone, string? userName, string? address,
                                                   string? provinceCity, string? district, string? wardCommune, DateTime? dateOfBirth)
        {
            try
            {
                var manager = await _context.Users
                    .Include(u => u.UserInformation)
                    .FirstOrDefaultAsync(u => u.UserId == userId && u.IsDelete!=true);

                if (manager == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy Manager với ID: {userId}");
                }
                var existPhoneOrEmail = await _context.Users.FirstOrDefaultAsync(c => c.UserId != userId && (c.Email==email || c.Phone==phone));
                if(existPhoneOrEmail != null)
                {
                    throw new InvalidOperationException("Email hoặc số điện thoại đã tồn tại tồn tại.");
                }
                // Update email 
                if (!string.IsNullOrWhiteSpace(email) && manager.Email != email)
                {
                    manager.Email = email;
                }

                // Update phone
                if (!string.IsNullOrWhiteSpace(phone))
                {
                    manager.Phone = phone;
                }

                // Update thông tin UserInformation nếu có
                if (manager.UserInformation != null)
                {
                    if (!string.IsNullOrWhiteSpace(userName))
                    manager.UserInformation.UserName = userName;
                    manager.UserInformation.Address = address;
                    manager.UserInformation.ProvinceCity = provinceCity;
                    manager.UserInformation.District = district;
                    manager.UserInformation.WardCommune = wardCommune;
                    manager.UserInformation.DateOfBirth = dateOfBirth;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }




    }
}
