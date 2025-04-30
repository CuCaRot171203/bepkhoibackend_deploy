using BepKhoiBackend.DataAccess.Abstract.OrderDetailAbstract;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Models.ExtendObjects;
using Microsoft.EntityFrameworkCore;

namespace BepKhoiBackend.DataAccess.Repository.OrderDetailRepository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        public readonly bepkhoiContext _context;

        public OrderDetailRepository(bepkhoiContext context)
        {
            _context = context;
        }

        // Task for get order detail by Id
        public async Task<OrderDetail?> GetOrderDetailByIdAsync(int orderDetailId)
        {
            return await _context.OrderDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);
        }

        // Task for create cancellation

        public async Task CreateOrderCancellationHistoryAsync(OrderCancellationHistory orderCancellationHistory)
        {
            _context.OrderCancellationHistories.Add(orderCancellationHistory);
            await _context.SaveChangesAsync();
        }


        // Task for delete order detail
        public async Task DeleteOrderDetailAsync(OrderDetail orderDetail)
        {
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
        }

        // Func to add orderdetail
        public async Task AddOrderDetailAsync(OrderDetail orderDetail)
        {
            await _context.OrderDetails.AddAsync(orderDetail);
            await _context.SaveChangesAsync();
        }

        // Func to update order detail
        public async Task UpdateOrderDetailAsync(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
            await _context.SaveChangesAsync();
        }

        // Func to get order detail by product id
        public async Task<OrderDetail?> GetOrderDetailByProductAsync(int orderId, int productId)
        {
            return await _context.OrderDetails
                .FirstOrDefaultAsync(od => od.OrderId == orderId && od.ProductId == productId && string.IsNullOrEmpty(od.ProductNote) && od.Status==false);
        }

        //Pham Son Tung
        //Func to turn all Order_detail status of an "Order_id" to "true" - API ConfirmOrderPos
        public async Task<bool> ConfirmOrderPosRepoAsync(int orderId)
        {
            try
            {
                var orderDetails = await _context.OrderDetails
                    .Where(od => od.OrderId == orderId)
                    .ToListAsync();

                if (!orderDetails.Any())
                {
                    return false; 
                }

                bool hasUpdated = false;

                foreach (var detail in orderDetails)
                {
                    if (detail.Status != true) 
                    {
                        detail.Status = true;
                        hasUpdated = true;
                    }
                }

                if (hasUpdated) 
                {
                    await _context.SaveChangesAsync();
                }

                return true; 
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Failed to update order details status in database.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating order details status.", ex);
            }
        }

        //Pham Son Tung
        //Func to check order exist
        public async Task<bool> CheckOrderExistRepoAsync(int orderId)
        {
            return await _context.Orders.AnyAsync(od => od.OrderId == orderId);
        }
        //Pham Son Tung
        //Func to update TotalQuantity and AmountDue of an Order
        public async Task UpdateOrderTotalsRepoAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    throw new Exception($"Order with ID {orderId} not found.");
                }

                order.TotalQuantity = order.OrderDetails.Sum(od => od.Quantity);
                order.AmountDue = order.OrderDetails.Sum(od => od.Quantity * od.Price);

                _context.Orders.Update(order);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Database update failed while updating totals for order ID {orderId}.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating totals for order ID {orderId}.", ex);
            }
        }


        //Pham Son Tung
        //split order API
        public async Task<bool> SplitOrderDetailRepoAsync(int sourceOrderId, int targetOrderId, List<SplitOrderPosExtendObject_ProductList> productList)
        {
            try
            {
                // Kiểm tra sự tồn tại của cả hai orderId
                bool sourceExists = await CheckOrderExistRepoAsync(sourceOrderId);
                bool targetExists = await CheckOrderExistRepoAsync(targetOrderId);
                if (!sourceExists || !targetExists)
                {
                    return false;
                }

                // Lấy danh sách OrderDetails của sourceOrderId
                var sourceOrderDetails = await _context.OrderDetails
                    .Where(od => od.OrderId == sourceOrderId)
                    .ToListAsync();

                if (sourceOrderDetails == null || sourceOrderDetails.Count == 0)
                {
                    throw new Exception("No order details found for the source order.");
                }

                foreach (var item in productList)
                {
                    int orderDetailId = item.Order_detail_id;
                    int productId = item.Product_id;
                    int quantityToMove = item.Quantity;

                    var sourceDetail = sourceOrderDetails.FirstOrDefault(od => od.OrderDetailId == orderDetailId && od.ProductId == productId);

                    if (sourceDetail == null)
                    {
                        throw new Exception($"Order detail with ID {orderDetailId} and product ID {productId} not found in source order.");
                    }

                    if (sourceDetail.Quantity < quantityToMove)
                    {
                        throw new Exception($"Insufficient quantity in source order detail ID {orderDetailId}.");
                    }

                    // Kiểm tra xem sản phẩm này đã có trong targetOrder chưa
                    var targetDetail = await _context.OrderDetails
                        .FirstOrDefaultAsync(od => od.OrderId == targetOrderId && od.ProductId == productId);

                    if (targetDetail != null && targetDetail.ProductNote == null && sourceDetail.ProductNote == null && targetDetail.Status == sourceDetail.Status)
                    {
                        // Nếu đã có, tăng số lượng
                        targetDetail.Quantity += quantityToMove;
                    }
                    else
                    {
                        // Nếu chưa có, tạo mới
                        var newOrderDetail = new OrderDetail
                        {
                            OrderId = targetOrderId,
                            ProductId = productId,
                            ProductName = sourceDetail.ProductName,
                            Quantity = quantityToMove,
                            Price = sourceDetail.Price,
                            ProductNote = sourceDetail.ProductNote,
                            Status = sourceDetail.Status
                        };
                        await _context.OrderDetails.AddAsync(newOrderDetail);
                    }

                    // Giảm số lượng của sản phẩm trong sourceOrder
                    sourceDetail.Quantity -= quantityToMove;
                    if (sourceDetail.Quantity == 0)
                    {
                        _context.OrderDetails.Remove(sourceDetail);
                    }
                }
                // Cập nhật lại TotalQuantity và AmountDue của cả hai order
                await UpdateOrderTotalsRepoAsync(sourceOrderId);
                await UpdateOrderTotalsRepoAsync(targetOrderId);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Failed to move order details due to a database error.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while moving order details.", ex);
            }
        }
        //ngocquan
        public async Task<List<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Include(od => od.Product) 
                .ToListAsync();
        }


        //Pham Son Tung
        public async Task<bool> CreateAndSplitOrderDetailRepoAsync(int sourceOrderId, int orderTypeId, int? roomId, int? shipperId, List<SplitOrderPosExtendObject_ProductList> productList)
        {
            try
            {
                // Kiểm tra sự tồn tại của sourceOrderId
                bool sourceExists = await CheckOrderExistRepoAsync(sourceOrderId);
                if (!sourceExists)
                {
                    return false;
                }

                // Lấy danh sách OrderDetails của sourceOrderId
                var sourceOrderDetails = await _context.OrderDetails
                    .Where(od => od.OrderId == sourceOrderId)
                    .ToListAsync();

                if (sourceOrderDetails == null || sourceOrderDetails.Count == 0)
                {
                    throw new Exception("No order details found for the source order.");
                }

                // Tạo đơn hàng mới
                var newOrder = new Order
                {
                    OrderTypeId = orderTypeId,
                    RoomId = (orderTypeId == 3) ? roomId : null, // Chỉ gán RoomId nếu là đơn ăn tại quán
                    ShipperId = (orderTypeId == 2) ? shipperId : null, // Chỉ gán ShipperId nếu là đơn ship
                    TotalQuantity = 0,
                    AmountDue = 0,
                    OrderStatusId = 1 // Trạng thái mặc định khi tạo mới
                };

                await _context.Orders.AddAsync(newOrder);
                await _context.SaveChangesAsync(); // Lưu lại để có OrderId

                int newOrderId = newOrder.OrderId;

                foreach (var item in productList)
                {
                    int orderDetailId = item.Order_detail_id;
                    int productId = item.Product_id;
                    int quantityToMove = item.Quantity;

                    var sourceDetail = sourceOrderDetails.FirstOrDefault(od => od.OrderDetailId == orderDetailId && od.ProductId == productId);
                    if (sourceDetail == null || sourceDetail.Quantity < quantityToMove)
                    {
                        throw new Exception($"Invalid product selection or insufficient quantity for OrderDetailId {orderDetailId}.");
                    }

                    // Thêm sản phẩm vào đơn mới
                    var newOrderDetail = new OrderDetail
                    {
                        OrderId = newOrderId,
                        ProductId = productId,
                        ProductName = sourceDetail.ProductName,
                        Quantity = quantityToMove,
                        Price = sourceDetail.Price,
                        ProductNote = sourceDetail.ProductNote,
                        Status = sourceDetail.Status
                    };
                    await _context.OrderDetails.AddAsync(newOrderDetail);

                    // Giảm số lượng trong đơn gốc
                    sourceDetail.Quantity -= quantityToMove;
                    if (sourceDetail.Quantity == 0)
                    {
                        _context.OrderDetails.Remove(sourceDetail);
                    }
                }
                // Cập nhật tổng số lượng và tổng tiền cho cả hai đơn
                await UpdateOrderTotalsRepoAsync(sourceOrderId);
                await UpdateOrderTotalsRepoAsync(newOrderId);
                return await _context.SaveChangesAsync() >0;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Failed to create and move order details due to a database error.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while creating and moving order details.", ex);
            }
        }
    }
}
