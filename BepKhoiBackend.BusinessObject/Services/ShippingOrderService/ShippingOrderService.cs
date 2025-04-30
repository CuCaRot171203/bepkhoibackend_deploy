using BepKhoiBackend.DataAccess.Repository.ShippingOrderRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepKhoiBackend.BusinessObject.dtos.ShippingOrderDto;

namespace BepKhoiBackend.BusinessObject.Services.ShippingOrderService
{
    public class ShippingOrderService : IShippingOrderService
    {
        private readonly IShippingOrderRepository _shippingOrderRepository;

        public ShippingOrderService(IShippingOrderRepository shippingOrderRepository)
        {
            _shippingOrderRepository = shippingOrderRepository;
        }

        public List<ShipperOrderDTO> GetShippersWithOrders()
        {
            var shippers = _shippingOrderRepository.GetShippers();

            return shippers.Select(shipper => new ShipperOrderDTO
            {
                UserId = shipper.UserId,
                UserName = shipper.UserInformation.UserName,
                OrderList = _shippingOrderRepository.GetOrdersByShipper(shipper.UserId)
                    .Select(o => new OrderDTO
                    {
                        OrderId = o.OrderId,
                        CreatedTime = o.CreatedTime,
                        OrderNote = o.OrderNote,
                        OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
                        {
                            OrderDetailId = od.OrderDetailId,
                            ProductName = od.Product.ProductName,
                            Quantity = od.Quantity,
                            Price = od.Price
                        }).ToList()
                    }).ToList()
            }).ToList();
        }
    }
}
