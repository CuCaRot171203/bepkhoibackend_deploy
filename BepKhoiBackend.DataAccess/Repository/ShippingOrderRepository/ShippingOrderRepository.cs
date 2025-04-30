using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace BepKhoiBackend.DataAccess.Repository.ShippingOrderRepository
{
    public class ShippingOrderRepository : IShippingOrderRepository
    {
        private readonly bepkhoiContext _context;

        public ShippingOrderRepository(bepkhoiContext context)
        {
            _context = context;
        }

        // Lấy danh sách Shipper (giả sử RoleId = 3 là shipper)
        public List<User> GetShippers()
        {
            return _context.Users
                .Include(u => u.UserInformation)
                .Where(u => u.RoleId == 3)
                .ToList();
        }

        // Lấy danh sách đơn hàng theo ShipperId và OrderStatusId = 1
        public List<Order> GetOrdersByShipper(int shipperId)
        {
            return _context.Orders
                .Where(o => o.OrderStatusId == 1 && o.ShipperId == shipperId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product) // Include tên sản phẩm
                .ToList();
        }
    }
}
