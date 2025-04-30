using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BepKhoiBackend.DataAccess.Repository.TakeAwayOrderRepository
{
    public class TakeAwayOrderRepository : ITakeAwayOrderRepository
    {
        private readonly bepkhoiContext _context;

        public TakeAwayOrderRepository(bepkhoiContext context)
        {
            _context = context;
        }

        public List<Order> GetTakeAwayOrders()
        {
            return _context.Orders
                .Where(o => o.OrderTypeId == 1) // Lọc đơn mang về
                .Include(o => o.Customer)       // Lấy thông tin khách hàng
                .Include(o => o.Shipper)        // Lấy thông tin nhân viên giao hàng
                .Include(o => o.OrderStatus)    // Lấy trạng thái đơn hàng
                .Include(o => o.OrderType)      // Lấy loại đơn hàng
                .Include(o => o.OrderDetails)   // Lấy danh sách chi tiết đơn hàng
                    .ThenInclude(od => od.Product) // Lấy tên sản phẩm từ Menu
                .ToList();
        }
    }
}
