using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Models.ExtendObjects;

namespace BepKhoiBackend.DataAccess.Abstract.OrderDetailAbstract
{
    public interface IOrderDetailRepository
    {
        Task<OrderDetail?> GetOrderDetailByIdAsync(int orderDetailId);
        Task CreateOrderCancellationHistoryAsync(OrderCancellationHistory orderCancellationHistory);
        Task DeleteOrderDetailAsync(OrderDetail orderDetail);
        Task AddOrderDetailAsync(OrderDetail orderDetail);
        Task UpdateOrderDetailAsync(OrderDetail orderDetail);
        Task<OrderDetail?> GetOrderDetailByProductAsync(int orderId, int productId);
        Task<bool> ConfirmOrderPosRepoAsync(int orderId);
        Task<bool> SplitOrderDetailRepoAsync(int sourceOrderId, int targetOrderId, List<SplitOrderPosExtendObject_ProductList> productList);
        Task<bool> CreateAndSplitOrderDetailRepoAsync(int sourceOrderId, int orderTypeId, int? roomId, int? shipperId, List<SplitOrderPosExtendObject_ProductList> productList);

        //ngocquan
        Task<List<OrderDetail>> GetByOrderIdAsync(int orderId);

    }
}
