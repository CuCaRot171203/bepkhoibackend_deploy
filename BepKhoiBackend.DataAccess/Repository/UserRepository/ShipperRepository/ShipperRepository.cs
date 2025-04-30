using BepKhoiBackend.DataAccess.Models;
using DocumentFormat.OpenXml.Math;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.UserRepository.ShipperRepository
{
    public class ShipperRepository : IShipperRepository
    {
        private readonly bepkhoiContext _context;

        public ShipperRepository(bepkhoiContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả shipper (RoleId = 3)
        public List<User> GetAllShippers()
        {
            return _context.Users
                .Include(u => u.UserInformation)
                .Include(u => u.Orders)
                .Where(u => u.RoleId == 3 && (u.IsDelete == false || u.IsDelete == null))
                .ToList();
        }

        // Lấy shipper theo ID
        public User? GetShipperById(int id)
        {
            return _context.Users
                .Include(u => u.UserInformation)
                .Include(u => u.Role)// Load thông tin người dùng
                .FirstOrDefault(u => u.UserId == id && u.RoleId == 3 && (u.IsDelete == false || u.IsDelete == null));
        }

        // Thêm mới Shipper
        public void CreateShipper(string email, string password, string phone, string userName)
        {
            // Tạo UserInformation trước
            var userInfo = new UserInformation
            {
                UserName = userName
            };

            _context.UserInformations.Add(userInfo);
            _context.SaveChanges(); // Lưu để lấy UserInformationId

            // Tạo User với UserInformationId vừa tạo
            var shipper = new User
            {
                Email = email,
                Password = password,
                Phone = phone,
                RoleId = 3, // Role = Shipper
                UserInformationId = userInfo.UserInformationId,
                Status = true, // Mặc định kích hoạt
                CreateDate = DateTime.UtcNow,
                IsVerify = false,
                IsDelete = false
            };

            _context.Users.Add(shipper);
            _context.SaveChanges();
        }

        // Cập nhật thông tin Shipper
        public bool UpdateShipper(int userId, string email, string phone, string userName, string address,
                                string provinceCity, string district, string wardCommune, DateTime? dateOfBirth)
        {
            var shipper = _context.Users
                .Include(u => u.UserInformation)
                .Include(u => u.Role)
                .FirstOrDefault(u => u.UserId == userId && u.Role.RoleName=="shipper");

            if (shipper == null)
            {
                return false; // Không tìm thấy shipper
            }
            var existPhoneOrEmail = _context.Users.FirstOrDefault(c => c.UserId != userId && (c.Email == email || c.Phone == phone));
            if (existPhoneOrEmail != null)
            {
                throw new InvalidOperationException("Email hoặc số điện thoại đã tồn tại tồn tại.");
            }
            // Kiểm tra nếu có thay đổi email thì đặt is_verify = false
            if (!string.IsNullOrEmpty(email) && shipper.Email != email)
            {
                shipper.Email = email;
                shipper.IsVerify = false; // Đánh dấu email chưa xác thực
            }

            // Cập nhật các thông tin cơ bản
            shipper.Phone = phone;

            if (shipper.UserInformation != null)
            {
                shipper.UserInformation.UserName = userName;
                shipper.UserInformation.Address = address;
                shipper.UserInformation.ProvinceCity = provinceCity;
                shipper.UserInformation.District = district;
                shipper.UserInformation.WardCommune = wardCommune;
                shipper.UserInformation.DateOfBirth = dateOfBirth;
            }

            _context.SaveChanges();
            return true;
        }



        // Xóa Shipper (Đánh dấu IsDelete = true)
        public void DeleteShipper(int userId)
        {
            var shipper = _context.Users
                .FirstOrDefault(u => u.UserId == userId && u.RoleId == 3);

            if (shipper != null)
            {
                shipper.IsDelete = true; // Đánh dấu là đã xóa
                _context.SaveChanges();
            }
        }
        //Lấy hóa đơn của Shipper
        public List<Invoice> GetShipperInvoices(int shipperId)
        {
            return _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.OrderType)
                .Include(i => i.PaymentMethod) // Lấy thêm PaymentMethod
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .Where(i => i.ShipperId == shipperId && i.OrderTypeId == 2)
                .ToList();
        }
        public List<User> GetShippers(string searchTerm = null, bool? status = null)
        {
            var query = _context.Users
                .Include(u => u.UserInformation)
                .Where(u => u.RoleId == 3 && u.IsDelete == false);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim(); // Loại bỏ khoảng trắng không cần thiết
                query = query.Where(u => EF.Functions.Like(u.UserInformation.UserName, $"%{searchTerm}%") ||
                                         EF.Functions.Like(u.Phone, $"%{searchTerm}%"));
            }

            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status.Value);
            }

            return query.OrderBy(u => u.Status).ToList();
        }


    }
}
