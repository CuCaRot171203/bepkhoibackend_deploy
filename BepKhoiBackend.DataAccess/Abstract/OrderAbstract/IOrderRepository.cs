using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Models.ExtendObjects;

namespace BepKhoiBackend.DataAccess.Abstract.OrderAbstract
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderPosAsync(Order order);
        Task<Order?> GetOrderByIdPosAsync(int orderId);
        Task<Order> UpdateOrderAsyncPos(Order order);
        Task<OrderDetail> GetOrderDetailByIdAsync(int orderDetailId);
        Task UpdateOrderDetailAsync(OrderDetail orderDetail);
        Task<Customer?> GetCustomerByIdAsync(int customerId);
        Task UpdateOrderAsync(Order order);
        Task<Menu?> GetProductByIdAsync(int productId);
        Task<bool> MoveOrderPosRepoAsync(int orderId, int orderTypeId, int? roomId, int? userId);
        Task<bool> CombineOrderPosRepoAsync(int firstOrderId, int secondOrderId);
        Task<IEnumerable<Order>> GetOrdersByTypePos(int? roomId, int? shipperId, int? orderTypeId);
        //-------------NgocQuan---------------//
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
        Task<Customer> GetCustomerIdByOrderIdAsync(int orderId);
        Task AssignCustomerToOrder(int orderId, int customerId);
        Task<bool> RemoveCustomerFromOrderAsync(int orderId);
        Task<Order> RemoveOrder(int orderId);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<List<Order>> GetAllAsync();
        Task<List<Order>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int? orderId);
        Task AddOrderDetailsAsync(List<OrderDetail> orderDetails);
        Task<Order> GetAllOrderData(int orderId);
        Task UpdateOrderAfterAddOrderDetailAsync(int orderId);
        Task DeleteOrderDetailAsync(int orderId, int orderDetailId);
        Task DeleteConfirmedOrderDetailAsync(int orderId, int orderDetailId, OrderCancellationHistory cancellation);
        Task<(int roomId, bool? isUse)> UpdateRoomIsUseByRoomIdAsync(int roomId);
        Task<Order?> GetFullOrderByIdAsync(int orderId);
        Task<bool> CreateDeliveryInformationAsync(int orderId, string receiverName, string receiverPhone, string receiverAddress, string? deliveryNote);
        Task<DeliveryInformation?> GetDeliveryInformationByOrderIdAsync(int orderId);
        Task<List<int>> GetOrderIdsForQrSiteAsync(int roomId, int customerId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<bool> UpdateOrderCustomerAsync(Order order);
        Task<bool> AddOrUpdateOrderDetailsAsync(Order order, List<OrderDetail> newDetails);
        (int roomId, bool? isUse) UpdateRoomIsUseByRoomId(int roomId);
        Task<List<OrderCancellationHistory>> GetOrderCancellationHistoryByIdAsync(int orderId);
        Task<DeliveryInformation?> GetDeliveryInformationByIdAsync(int DeliveryInformationId);
        Task<Order?> GetOrderFullInforByIdAsync(int orderId);
        Task<List<Order>> FilterOrderManagerAsync(FilterOrderManager dto);
    }
}
