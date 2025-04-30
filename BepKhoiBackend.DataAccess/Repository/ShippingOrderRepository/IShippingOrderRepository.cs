using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.ShippingOrderRepository
{
    public interface IShippingOrderRepository
    {
        List<User> GetShippers();
        List<Order> GetOrdersByShipper(int shipperId);
    }
}
