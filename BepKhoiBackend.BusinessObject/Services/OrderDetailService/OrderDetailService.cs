using AutoMapper;
using BepKhoiBackend.BusinessObject.Abstract.OrderDetailAbstract;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.DataAccess.Abstract.OrderAbstract;
using BepKhoiBackend.DataAccess.Abstract.OrderDetailAbstract;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Models.ExtendObjects;
using BepKhoiBackend.DataAccess.Repository.OrderRepository;
using BepKhoiBackend.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.OrderDetailService
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;
        private readonly bepkhoiContext _context;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository, IMapper mapper, bepkhoiContext context)
        {
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> CancelOrderDetailAsync(CancelOrderDetailRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var orderDetail = await _orderDetailRepository.GetOrderDetailByIdAsync(request.OrderDetailId);

                if(orderDetail == null)
                {
                    throw new KeyNotFoundException("Order detail not found.");
                }

                if (orderDetail.Status != true)
                {
                    throw new ArgumentException("Order detail can only be canceled after being sent to the kitchen.");
                }

                if (orderDetail.Quantity < request.Quantity)
                {
                    throw new ArgumentException("Cancel quantity cannot exceed the current order quantity.");
                }

                // Update OrderDetail
                orderDetail.Quantity -= request.Quantity;

                if (orderDetail.Quantity == 0)
                {
                    _context.OrderDetails.Remove(orderDetail);
                }
                else
                {
                    _context.OrderDetails.Update(orderDetail);
                }

                await _context.SaveChangesAsync();

                // Create OrderCancellationHistory
                var cancellationHistory = new OrderCancellationHistory
                {
                    OrderId = request.OrderId,
                    CashierId = request.CashierId,
                    ProductId = orderDetail.ProductId,
                    Quantity = request.Quantity,
                    Reason = request.Reason
                };

                await _orderDetailRepository.CreateOrderCancellationHistoryAsync(cancellationHistory);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Method to remove
        public async Task<bool> RemoveOrderDetailAsync(RemoveOrderDetailRequest request)
        {
            try
            {
                var orderDetail = await _orderDetailRepository.GetOrderDetailByIdAsync(request.OrderDetailId);

                //Check null
                if(orderDetail == null)
                {
                    //throw new KeyNotFoundException($"Order detail with ID {request.OrderDetailId} not found.");
                    return false;
                }

                if(orderDetail.Status == true)
                {
                    throw new ArgumentException("Order detail cannot be removed after being sent to the kitchen.");
                }

                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> AddNoteToOrderDetailAsync(AddNoteToOrderDetailRequest request)
        {
            try
            {
                var orderDetail = await _orderDetailRepository.GetOrderDetailByIdAsync(request.OrderDetailId);

                // Check null
                if (orderDetail == null)
                {
                    throw new KeyNotFoundException("Order detail not found.");
                }

                // check matched
                if(orderDetail.OrderId != request.OrderId)
                {
                    throw new ArgumentException("Order ID mismatch.");
                }

                orderDetail.ProductNote = request.Note;
                _context.OrderDetails.Update(orderDetail);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                throw;
            }
        }

        //Pham Son Tung
        //Func to turn all Order_detail status of an "Order_id" to "true" - API ConfirmOrderPos
        public async Task<bool> ConfirmOrderPosServiceAsync(int orderId)
        {
            if (orderId <= 0)
            {
                throw new ArgumentException("Invalid order ID. It must be greater than 0.");
            }

            try
            {
                return await _orderDetailRepository.ConfirmOrderPosRepoAsync(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while confirming order.", ex);
            }
        }

        //Pham Son Tung 
        //Func for SplitOrderPos(Tách đơn) API
        public async Task<bool> SplitOrderPosServiceAsync(SplitOrderPosRquest request)
        {
            // Mapping từ DTO sang object của tầng Repository
            var productListRepo = request.Product.Select(p => new SplitOrderPosExtendObject_ProductList
            {
                Order_detail_id = p.Order_detail_id,
                Product_id = p.Product_id,
                Quantity = p.Quantity
            }).ToList();
            if (request.CreateNewOrder)
            {
                return await _orderDetailRepository.CreateAndSplitOrderDetailRepoAsync(
                    request.OrderId,
                    request.OrderTypeId,
                    request.RoomId,
                    request.ShipperId,
                    productListRepo);
            }
            else
            {
                if (request.SplitTo == null)
                {
                    throw new ArgumentException("SplitTo must be provided when CreateNewOrder is false.");
                }
                return await _orderDetailRepository.SplitOrderDetailRepoAsync(
                    request.OrderId,
                    request.SplitTo.Value,
                    productListRepo);
            }
        }
        public async Task<ResultWithList<OrderDetailDto>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            try
            {
                var details = await _orderDetailRepository.GetByOrderIdAsync(orderId);

                var result = details.Select(d => new OrderDetailDto
                {
                    OrderDetailId = d.OrderDetailId,
                    OrderId = d.OrderId,
                    ProductId = d.ProductId,
                    ProductName = d.Product?.ProductName ?? "Unknown",
                    Quantity = d.Quantity,
                    Price = d.Price,
                    ProductNote = d.ProductNote
                }).ToList();

                return new ResultWithList<OrderDetailDto>
                {
                    IsSuccess = true,
                    Message = "Fetched order details successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ResultWithList<OrderDetailDto>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
    }
}
