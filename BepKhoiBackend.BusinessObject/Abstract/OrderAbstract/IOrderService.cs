using BepKhoiBackend.BusinessObject.dtos.CustomerDto;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDto.PaymentDto;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Abstract.OrderAbstract
{
    public interface IOrderService
    {

        Task<(OrderDto orderDto, int? roomId, bool? isUse)> CreateNewOrderAsync(CreateOrderRequestDto request);
        Task<OrderDto> AddOrderNoteToOrderPosAsync(AddNoteRequest request);
        Task<OrderDetailDto> UpdateOrderDetailQuantiyPosAsync(UpdateOrderDetailQuantityRequest request);
        Task<bool> AddCustomerToOrderAsync(AddCustomerToOrderRequest request);
        Task<bool> AddProductToOrderAsync(AddProductToOrderRequest request);
        Task<bool> ChangeOrderTypeServiceAsync(MoveOrderPosRequestDto request);
        Task<bool> CombineOrderPosServiceAsync(CombineOrderPosRequestDto request);
        Task<IEnumerable<OrderDtoPos>> GetOrdersByTypePosAsync(int? roomId, int? shipperId, int? orderTypeId);
        Task<CustomerPosDto> GetCustomerIdByOrderIdAsync(int orderId);
        Task AssignCustomerToOrderAsync(int orderId, int customerId);
        Task<bool> RemoveCustomerFromOrderAsync(int orderId);
        Task<(OrderDto orderDto, int? roomId, bool? isUse)> RemoveOrderById(int orderId);
        Task<IEnumerable<OrderDetailDtoPos>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<ResultWithList<OrderDto>> GetAllOrdersAsync();
        Task<ResultWithList<OrderDto>> FilterOrdersByDateAsync(DateTime? fromDate, DateTime? toDate, int? orderId);

        Task<OrderGeneralDataPosDto> GetOrderGeneralDataPosAsync(int orderId);
        Task DeleteOrderDetailAsync(int orderId, int orderDetailId);
        Task DeleteConfirmedOrderDetailAsync(DeleteConfirmedOrderDetailRequestDto request);
        Task<OrderPaymentDto?> GetOrderPaymentDtoByIdAsync(int orderId);
        Task<bool> CreateDeliveryInformationServiceAsync(DeliveryInformationCreateDto dto);
        Task<DeliveryInformationDto?> GetDeliveryInformationByOrderIdAsync(int orderId);
        Task<List<int>> GetOrderIdsForQrSiteAsync(int roomId, int customerId);
        Task<bool> UpdateOrderWithDetailsAsync(OrderUpdateDTO dto);
        Task<List<OrderCancellationHistoryDto>> GetOrderCancellationHistoryByIdAsync(int orderCancellationHistoryId);
        Task<DeliveryInformation?> GetDeliveryInformationByIdAsync(int DeliveryInformationId);
        Task<OrderFullInForDto> GetOrderFullInforByIdAsync(int orderId);
        Task<ResultWithList<OrderDto>> FilterOrderManagerAsync(FilterOrderManagerDto dto);

    }
}
