using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.UserRepository.CashierRepository
{
    public class CashierRepository : ICashierRepository
    {
        private readonly bepkhoiContext _context;

        public CashierRepository(bepkhoiContext context)
        {
            _context = context;
        }

        public List<User> GetAllCashiers()
        {
            return _context.Users
                .Include(u => u.UserInformation) // Load thông tin người dùng
                .Include(u => u.InvoiceCashiers)
                .Where(u => u.RoleId == 2 && (u.IsDelete == false || u.IsDelete == null))
                .ToList();
        }

        public User GetCashierById(int id)
        {
            return _context.Users
                .Include(u => u.UserInformation)
                .Include(u => u.Role)// Load thông tin người dùng
                .FirstOrDefault(u => u.UserId == id && u.RoleId == 2 && (u.IsDelete == false || u.IsDelete == null));
        }

        // Thêm mới Cashier
        public void CreateCashier(string email, string password, string phone, string userName)
        {
            var userInfo = new UserInformation
            {
                UserName = userName
            };

            _context.UserInformations.Add(userInfo);
            _context.SaveChanges();

            var cashier = new User
            {
                Email = email,
                Password = password,
                Phone = phone,
                RoleId = 2, // Role = Cashier
                UserInformationId = userInfo.UserInformationId,
                Status = true,
                CreateDate = DateTime.UtcNow,
                IsVerify = false,
                IsDelete = false
            };

            _context.Users.Add(cashier);
            _context.SaveChanges();
        }

        // Cập nhật thông tin Cashier
        public async Task<bool> UpdateCashier(int userId, string email, string phone, string userName, string address,
                                  string provinceCity, string district, string wardCommune, DateTime? dateOfBirth)
        {
            try
            {
                var cashier = _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.UserInformation)
                    .FirstOrDefault(u => u.UserId == userId && u.Role.RoleName == "cashier");

                if (cashier == null)
                {
                    return false;
                }
                var existPhoneOrEmail = await _context.Users.FirstOrDefaultAsync(c => c.UserId != userId && (c.Email == email || c.Phone == phone));
                if (existPhoneOrEmail != null)
                {
                    throw new InvalidOperationException("Email hoặc số điện thoại đã tồn tại tồn tại.");
                }
                if (!string.IsNullOrEmpty(email) && cashier.Email != email)
                {
                    cashier.Email = email;
                    cashier.IsVerify = false;
                }
                cashier.Phone = phone;
                if (cashier.UserInformation != null)
                {
                    cashier.UserInformation.UserName = userName;
                    cashier.UserInformation.Address = address;
                    cashier.UserInformation.ProvinceCity = provinceCity;
                    cashier.UserInformation.District = district;
                    cashier.UserInformation.WardCommune = wardCommune;
                    cashier.UserInformation.DateOfBirth = dateOfBirth;
                }

                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Xóa Cashier (Đánh dấu IsDelete = true)
        public void DeleteCashier(int userId)
        {
            var cashier = _context.Users
                .FirstOrDefault(u => u.UserId == userId && u.RoleId == 2);

            if (cashier != null)
            {
                cashier.IsDelete = true;
                _context.SaveChanges();
            }
        }

        // Lấy hóa đơn của Cashier
        public List<Invoice> GetCashierInvoices(int cashierId)
        {
            return _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.OrderType)
                .Include(i => i.PaymentMethod)
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .Where(i => i.CashierId == cashierId && i.OrderTypeId == 2)
                .ToList();
        }
        public List<User> GetCashiers(string? searchTerm, bool? status)
        {
            var query = _context.Users
                .Include(u => u.UserInformation)
                .Where(u => u.RoleId == 2 && u.IsDelete == false);

            // Nếu có searchTerm, lọc theo tên hoặc số điện thoại
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(u =>
                    EF.Functions.Like(u.UserInformation.UserName, $"%{searchTerm}%") ||
                    EF.Functions.Like(u.Phone, $"%{searchTerm}%"));
            }

            // Nếu có status, lọc theo trạng thái
            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status.Value);
            }

            return query.OrderBy(u => u.Status).ToList();
        }

    }
}
