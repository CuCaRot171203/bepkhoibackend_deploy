using BepKhoiBackend.BusinessObject.dtos.TakeAwayOrderDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.TakeAwayOrderService
{
    public interface ITakeAwayOrderService
    {
        List<TakeAwayOrderDTO> GetTakeAwayOrders();
    }
}
