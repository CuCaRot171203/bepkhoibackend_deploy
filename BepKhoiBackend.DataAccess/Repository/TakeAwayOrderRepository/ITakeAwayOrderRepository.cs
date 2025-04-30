using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.DataAccess.Repository.TakeAwayOrderRepository
{
    public interface ITakeAwayOrderRepository
    {
        List<Order> GetTakeAwayOrders(); 
    }
}
