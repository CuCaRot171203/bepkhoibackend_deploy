using BepKhoiBackend.BusinessObject.dtos.TakeAwayOrderDto;
using BepKhoiBackend.DataAccess.Repository.TakeAwayOrderRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.TakeAwayOrderService
{
    public class TakeAwayOrderService : ITakeAwayOrderService
    {
        private readonly ITakeAwayOrderRepository _takeAwayOrderRepository;

        public TakeAwayOrderService(ITakeAwayOrderRepository takeAwayOrderRepository)
        {
            _takeAwayOrderRepository = takeAwayOrderRepository;
        }

        public List<TakeAwayOrderDTO> GetTakeAwayOrders()
        {
            var orders = _takeAwayOrderRepository.GetTakeAwayOrders();

            return orders.Select(o => new TakeAwayOrderDTO
            {
                OrderId = o.OrderId,
                CustomerName = o.Customer?.CustomerName, // Lấy tên khách hàng
                OrderStatusName = o.OrderStatus.OrderStatusTitle, // Lấy tên trạng thái
                OrderTypeName = o.OrderType.OrderTypeTitle, // Lấy tên loại đơn hàng
                CreatedTime = o.CreatedTime,
                TotalQuantity = o.TotalQuantity,
                AmountDue = o.AmountDue,
                OrderNote = o.OrderNote,
                OrderDetails = o.OrderDetails.Select(od => new TakeAwayOrderDetailDTO
                {
                    OrderDetailId = od.OrderDetailId,
                    ProductName = od.Product.ProductName, // Lấy tên sản phẩm
                    Quantity = od.Quantity,
                    Price = od.Price,
                    ProductNote = od.ProductNote
                }).ToList()
            }).ToList();
        }
    }
}
