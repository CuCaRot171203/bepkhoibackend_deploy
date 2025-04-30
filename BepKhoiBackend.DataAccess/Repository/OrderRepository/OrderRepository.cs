using BepKhoiBackend.DataAccess.Abstract.OrderAbstract;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Models.ExtendObjects;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace BepKhoiBackend.DataAccess.Repository.OrderRepository
{
    public class OrderRepository : IOrderRepository
    {
        public readonly bepkhoiContext _context;

        public OrderRepository(bepkhoiContext context)
        {
            _context = context;
        }

        //Task for Create Order
        public async Task<Order> CreateOrderPosAsync(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return order;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving the order to database.", ex);
            }
        }

        //Update isUse value
        public async Task<(int roomId, bool? isUse)> UpdateRoomIsUseByRoomIdAsync(int roomId)
        {
            try
            {
                var room = await _context.Rooms
                    .Include(r => r.Orders)
                    .FirstOrDefaultAsync(r => r.RoomId == roomId);

                if (room == null)
                {
                    throw new KeyNotFoundException("Room not found.");
                }
                bool hasActiveOrder = room.Orders.Any(order => order.OrderStatusId == 1);
                room.IsUse = hasActiveOrder;
                _context.Rooms.Update(room);
                await _context.SaveChangesAsync();
                return (room.RoomId, room.IsUse);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public (int roomId, bool? isUse) UpdateRoomIsUseByRoomId(int roomId)
        {
            try
            {
                var room = _context.Rooms
                    .Include(r => r.Orders)
                    .FirstOrDefault(r => r.RoomId == roomId); // Dùng FirstOrDefault (sync)

                if (room == null)
                {
                    throw new KeyNotFoundException("Room not found.");
                }

                bool hasActiveOrder = room.Orders.Any(order => order.OrderStatusId == 1);
                room.IsUse = hasActiveOrder;

                _context.Rooms.Update(room);
                _context.SaveChanges(); // Dùng SaveChanges (sync)

                return (room.RoomId, room.IsUse);
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Get order by Id for Pos site
        public async Task<Order?> GetOrderByIdPosAsync(int orderId)
        {
            return await _context.Orders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        // Function to update order for Pos site
        public async Task<Order> UpdateOrderAsyncPos(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        // Function to get Order Detail by Id for pos site
        public async Task<OrderDetail> GetOrderDetailByIdAsync(int orderDetailId)
        {
            return await _context.OrderDetails.AsNoTracking().FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);
        }

        // Function to Update Order Detail 
        public async Task UpdateOrderDetailAsync(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
            await _context.SaveChangesAsync();
        }

        // Func to get customer by Id
        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        // Pham Son Tung - Func to update order
        public async Task UpdateOrderAsync(Order order)
        {
            try
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
            catch(Exception)
            {
                throw;
            }
        }

        // Func to get product by Id
        public async Task<Menu?> GetProductByIdAsync(int productId)
        {
            return await _context.Menus
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }



        //Pham Son Tung
        //Repository function for MoveOrderPos API
        public async Task<bool> MoveOrderPosRepoAsync(int orderId, int orderTypeId, int? roomId, int? userId)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {orderId} not found.");
                }

                if (orderTypeId != 1 && orderTypeId != 2 && orderTypeId != 3)
                {
                    throw new ArgumentException("Invalid order type. Must be 1 (Takeaway), 2 (Ship), or 3 (Dine-in).");
                }

                if (orderTypeId == 3)
                {
                    var roomExists = await _context.Rooms.AnyAsync(r => r.RoomId == roomId);
                    if (!roomExists)
                    {
                        throw new KeyNotFoundException($"Room with ID {roomId} does not exist.");
                    }
                    order.RoomId = roomId;
                    order.ShipperId = null;
                }
                else
                {
                    order.RoomId = null;
                }

                if (orderTypeId == 2)
                {
                    var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
                    if (!userExists)
                    {
                        throw new KeyNotFoundException($"User (Shipper) with ID {userId} does not exist.");
                    }
                    order.ShipperId = userId;
                }
                else
                {
                    order.ShipperId = null;
                }

                order.OrderTypeId = orderTypeId;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the order type.", ex);
            }
        }


        //Pham Son Tung
        //Repository Func for CombineOrderPos API
        public async Task<bool> CombineOrderPosRepoAsync(int firstOrderId, int secondOrderId)
        {
            try
            {
                // Kiểm tra cả hai đơn hàng có tồn tại không
                var firstOrder = await _context.Orders.FindAsync(firstOrderId);
                var secondOrder = await _context.Orders.FindAsync(secondOrderId);

                if (firstOrder == null)
                    throw new KeyNotFoundException($"Order with ID {firstOrderId} not found.");
                if (secondOrder == null)
                    throw new KeyNotFoundException($"Order with ID {secondOrderId} not found.");

                // Lấy danh sách chi tiết đơn hàng từ cả hai đơn
                var firstOrderDetails = await _context.OrderDetails
                    .Where(od => od.OrderId == firstOrderId)
                    .ToListAsync();

                var secondOrderDetails = await _context.OrderDetails
                    .Where(od => od.OrderId == secondOrderId)
                    .ToListAsync();

                if (!firstOrderDetails.Any())
                    throw new InvalidOperationException($"Order with ID {firstOrderId} has no order details to transfer.");

                foreach (var sourceDetail in firstOrderDetails.ToList()) // clone tránh modify khi đang loop
                {
                    var matchingTarget = secondOrderDetails.FirstOrDefault(target =>
                        target.ProductId == sourceDetail.ProductId &&
                        target.ProductNote == null &&
                        sourceDetail.ProductNote == null &&
                        target.Status == sourceDetail.Status
                    );

                    if (matchingTarget != null)
                    {
                        // Gộp số lượng nếu trùng và thỏa điều kiện
                        matchingTarget.Quantity += sourceDetail.Quantity;

                        // Xóa detail gốc trong đơn hàng đầu
                        _context.OrderDetails.Remove(sourceDetail);
                    }
                    else
                    {
                        // Tạo mới OrderDetail trong đơn hàng đích
                        var newOrderDetail = new OrderDetail
                        {
                            OrderId = secondOrderId,
                            ProductId = sourceDetail.ProductId,
                            ProductName = sourceDetail.ProductName,
                            Quantity = sourceDetail.Quantity,
                            Price = sourceDetail.Price,
                            ProductNote = sourceDetail.ProductNote,
                            Status = sourceDetail.Status
                        };
                        await _context.OrderDetails.AddAsync(newOrderDetail);
                        _context.OrderDetails.Remove(sourceDetail); // Xóa khỏi đơn cũ
                    }
                }
                // Lưu thay đổi
                await _context.SaveChangesAsync();
                // Cập nhật lại tổng số lượng và thành tiền của cả hai đơn
                await UpdateOrderAfterUpdateOrderDetailAsync(firstOrderId);
                await UpdateOrderAfterUpdateOrderDetailAsync(secondOrderId);

                return true;
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException("Resource not found.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Business logic error.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while combining orders.", ex);
            }
        }


        //Pham Son Tung
        public async Task<IEnumerable<Order>> GetOrdersByTypePos(int? roomId, int? shipperId, int? orderTypeId)
        {
            if (!orderTypeId.HasValue)
                throw new ArgumentException("orderTypeId is required.");

            IQueryable<Order> query = _context.Orders
                .AsNoTracking();

            try
            {
                switch (orderTypeId)
                {
                    case 1:
                        query = query.Where(o => o.OrderStatusId == 1 && o.OrderTypeId == 1);
                        break;

                    case 2:
                        if (!shipperId.HasValue)
                            throw new ArgumentException("shipperId is required for orderTypeId = 2.");
                        query = query.Where(o => o.OrderStatusId == 1 && o.ShipperId == shipperId.Value);
                        break;

                    case 3:
                        if (!roomId.HasValue)
                            throw new ArgumentException("roomId is required for orderTypeId = 3.");
                        query = query.Where(o => o.OrderStatusId == 1 && o.RoomId == roomId.Value);
                        break;

                    default:
                        throw new ArgumentException("Invalid orderTypeId. Accepted values: {1, 2, 3}.");
                }

                // Thực hiện truy vấn và trả về kết quả
                return await query.ToListAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Bắt lỗi liên quan đến cập nhật database
                throw new Exception("Database update error occurred while fetching orders.", dbEx);
            }
            catch (Exception ex)
            {
                // Bắt tất cả các lỗi khác và ném lỗi tùy chỉnh
                throw new Exception("An error occurred while fetching orders.", ex);
            }
        }

        //Pham Son Tung
        public async Task<List<int>> GetOrderIdsForQrSiteAsync(int roomId, int customerId)
        {
            try
            {
                return await _context.Orders
                    .AsNoTracking()
                    .Where(o => o.RoomId == roomId && o.CustomerId == customerId && o.OrderStatusId == 1)
                    .Select(o => o.OrderId)
                    .ToListAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException("Database update error occurred while fetching order IDs.", dbEx);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //-------------NgocQuan---------------//
        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<Customer> GetCustomerIdByOrderIdAsync(int orderId)
        {
            try{
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {orderId} was not found at repository.");
                }
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == order.CustomerId);
                if (customer == null)
                {
                    throw new KeyNotFoundException($"Customer with ID {order.CustomerId} was not found at repository.");
                }
                return customer;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AssignCustomerToOrder(int orderId, int customerId)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {orderId} was not found.");
                }

                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    throw new KeyNotFoundException($"Customer with ID {customerId} was not found.");
                }

                order.CustomerId = customerId;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Database update error occurred while assigning customer to order.", dbEx);
            }
            catch (DbException dbEx)
            {
                throw new Exception("A database error occurred while assigning customer to order.", dbEx);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Pham Son Tung
        public async Task<bool> RemoveCustomerFromOrderAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    return false; 
                }
                order.CustomerId = null;
                await _context.SaveChangesAsync();
                return true; 
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Pham Son Tung
        public async Task<Order> RemoveOrder(int orderId)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {orderId} not found.");
                }
                order.OrderStatusId = 3;

                _context.Orders.Update(order);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return order;
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                throw new InvalidOperationException("Concurrency error occurred while updating the order.", dbEx);
            }
            catch (DbUpdateException dbEx)
            {
                throw new InvalidOperationException("Database error occurred while removing the order.", dbEx);
            }
            catch
            {
                throw;
            }
        }

        //Pham Son Tung
        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            try
            {
                var orderDetails = await _context.OrderDetails
                    .AsNoTracking()
                    .Where(od => od.OrderId == orderId)
                    .ToListAsync();

                return orderDetails;
            }
            catch (ArgumentNullException argEx)
            {
                throw new ArgumentException("Order ID must not be null.", argEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching order details by orderId.", ex);
            }
        }


        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<List<Order>> GetByDateRangeAsync(DateTime? fromDate = null, DateTime? toDate = null, int? orderId = null)
        {
            var query = _context.Orders.AsQueryable();

            // Apply date range filter if provided
            if (fromDate.HasValue)
            {
                query = query.Where(o => o.CreatedTime >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                // Add one day to include the entire toDate
                query = query.Where(o => o.CreatedTime <= toDate.Value.AddDays(1).AddTicks(-1));
            }

            if (orderId.HasValue)
            {
                query = query.Where(o => o.OrderId == orderId.Value);
            }

            // Add ordering and select only necessary fields
            query = query
                .OrderBy(o => o.CreatedTime)
                .AsNoTracking();

            return await query.ToListAsync();
        }

        //public async Task AddOrderAsync(Order order)
        //{
        //    await _context.Orders.AddAsync(order);
        //    await _context.SaveChangesAsync();
        //}
        //Pham Son Tung
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            try
            {
                return await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderId == orderId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Pham Son Tung
        public async Task<bool> UpdateOrderCustomerAsync(Order order)
        {
            try
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException("Failed to update order: " + ex.InnerException?.Message, ex);
            }
        }
        //Pham Son Tung
        public async Task<bool> AddOrUpdateOrderDetailsAsync(Order order, List<OrderDetail> newDetails)
        {
            try
            {
                if (order.OrderDetails == null)
                {
                    order.OrderDetails = new List<OrderDetail>();
                }
                var validProductIds = await _context.Menus
                     .Where(m => m.Status == true && m.IsAvailable==true && m.IsDelete != true)
                     .Select(m => m.ProductId)
                     .ToListAsync();
                var hasInvalid = newDetails.Any(detail => !validProductIds.Contains(detail.ProductId));
                if (hasInvalid)
                {
                    throw new ArgumentException("Product Detail list not valid");
                }
                foreach (var newDetail in newDetails)
                {
                    var existingDetails = order.OrderDetails
                        .Where(od => od.ProductId == newDetail.ProductId)
                        .ToList();

                    // Check if note exists → create new
                    if (!string.IsNullOrWhiteSpace(newDetail.ProductNote))
                    {
                        _context.OrderDetails.Add(newDetail);
                        continue;
                    }

                    var sameWithoutNote = existingDetails
                        .FirstOrDefault(od => string.IsNullOrEmpty(od.ProductNote));

                    if (sameWithoutNote != null)
                    {
                        if (sameWithoutNote.Status == false)
                        {
                            sameWithoutNote.Quantity += newDetail.Quantity;
                            _context.OrderDetails.Update(sameWithoutNote);
                        }
                        else
                        {
                            _context.OrderDetails.Add(newDetail);
                        }
                    }
                    else
                    {
                        _context.OrderDetails.Add(newDetail);
                    }
                }

                await _context.SaveChangesAsync();
                await UpdateOrderAfterUpdateOrderDetailAsync(order.OrderId);
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException("Failed to update order details: " + ex.InnerException?.Message, ex);
            }
        }


        public async Task AddOrderDetailsAsync(List<OrderDetail> orderDetails)
        {
            await _context.OrderDetails.AddRangeAsync(orderDetails);
            await _context.SaveChangesAsync();
        }


        //Pham Son Tung
        public async Task<Order> GetAllOrderData(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {orderId} was not found.");
                }
                return order;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("A database update error occurred while retrieving the order.", dbEx);
            }
            catch (InvalidOperationException invalidOpEx)
            {
                throw new Exception("An invalid operation occurred while retrieving the order.", invalidOpEx);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Pham Son Tung
        public async Task UpdateOrderAfterAddOrderDetailAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }
            order.TotalQuantity = order.OrderDetails.Sum(od => od.Quantity);
            order.AmountDue = order.OrderDetails.Sum(od => od.Quantity * od.Price);
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        //Pham Son Tung
        public async Task UpdateOrderAfterDeleteOrderDetailAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }
            order.TotalQuantity = order.OrderDetails.Sum(od => od.Quantity);
            order.AmountDue = order.OrderDetails.Sum(od => od.Quantity * od.Price);
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        //Pham Son Tung
        public async Task UpdateOrderAfterUpdateOrderDetailAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }
            order.TotalQuantity = order.OrderDetails.Sum(od => od.Quantity);
            order.AmountDue = order.OrderDetails.Sum(od => od.Quantity * od.Price);
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        //Pham Son Tung
        public async Task DeleteOrderDetailAsync(int orderId, int orderDetailId)
        {
            try
            {
                var orderDetail = await _context.OrderDetails
                    .FirstOrDefaultAsync(od => od.OrderId == orderId && od.OrderDetailId == orderDetailId);

                if (orderDetail == null)
                {
                    throw new KeyNotFoundException($"OrderDetail with ID {orderDetailId} for Order {orderId} not found.");
                }
                if (orderDetail.Status == true)
                {
                    throw new InvalidOperationException($"Can not delete OrderDetail ID {orderDetailId} because status is true.");
                }
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
                await UpdateOrderAfterDeleteOrderDetailAsync(orderId);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("A concurrency error occurred while deleting the OrderDetail.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while updating the database during deletion.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("An unexpected error occurred during deletion.", ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Pham Son Tung
        public async Task DeleteConfirmedOrderDetailAsync(int orderId, int orderDetailId, OrderCancellationHistory cancellation)
        {
            try
            {
                var orderDetail = await _context.OrderDetails
                    .FirstOrDefaultAsync(od => od.OrderId == orderId && od.OrderDetailId == orderDetailId);

                if (orderDetail == null)
                {
                    throw new KeyNotFoundException($"OrderDetail with ID {orderDetailId} for Order {orderId} not found.");
                }

                // Gán lại thông tin từ OrderDetail nếu cần
                cancellation.OrderId = orderId;
                cancellation.ProductId = orderDetail.ProductId;
                cancellation.Quantity = orderDetail.Quantity;

                await _context.OrderCancellationHistories.AddAsync(cancellation);

                _context.OrderDetails.Remove(orderDetail);

                await _context.SaveChangesAsync();

                await UpdateOrderAfterDeleteOrderDetailAsync(orderId);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("A concurrency error occurred while deleting the confirmed OrderDetail.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("A database error occurred while deleting the confirmed OrderDetail.", ex);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Pham Son Tung
        public async Task<Order?> GetFullOrderByIdAsync(int orderId)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product) 
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
            }
            catch (SqlException sqlEx)
            {
                // Lỗi liên quan đến kết nối SQL
                throw new Exception($"Database connect error: {sqlEx.Message}", sqlEx);
            }
            catch (DbException dbEx)
            {
                // Lỗi chung liên quan đến truy vấn CSDL
                throw new Exception($"Database connect error: {dbEx.Message}", dbEx);
            }
            catch (Exception ex)
            {
                // Bắt tất cả lỗi còn lại
                throw new Exception($"Undefined Error: {ex.Message}", ex);
            }
        }

        //Pham Son Tung
        public async Task<bool> CreateDeliveryInformationAsync(int orderId, string receiverName, string receiverPhone, string receiverAddress, string? deliveryNote)
        {
            // Kiểm tra OrderId có tồn tại không
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Can not find order with ID {orderId}.");
            }
            try
            {
                if (order.DeliveryInformationId.HasValue)
                {
                    var existingDeliveryInfo = await _context.DeliveryInformations.FindAsync(order.DeliveryInformationId.Value);

                    if (existingDeliveryInfo != null)
                    {
                        // Cập nhật thông tin giao hàng
                        existingDeliveryInfo.ReceiverName = receiverName;
                        existingDeliveryInfo.ReceiverPhone = receiverPhone;
                        existingDeliveryInfo.ReceiverAddress = receiverAddress;
                        existingDeliveryInfo.DeliveryNote = deliveryNote;

                        _context.DeliveryInformations.Update(existingDeliveryInfo);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                }
                var deliveryInfo = new DeliveryInformation
                {
                    ReceiverName = receiverName,
                    ReceiverPhone = receiverPhone,
                    ReceiverAddress = receiverAddress,
                    DeliveryNote = deliveryNote
                };

                _context.DeliveryInformations.Add(deliveryInfo);
                await _context.SaveChangesAsync();
                order.DeliveryInformationId = deliveryInfo.DeliveryInformationId;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException("Database error.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Undefined error has been occur.", ex);
            }
        }

        //Phạm Sơn Tùng
        public async Task<DeliveryInformation?> GetDeliveryInformationByOrderIdAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.DeliveryInformation)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    throw new InvalidOperationException($"Cannot find order with ID {orderId}.");
                }

                return order.DeliveryInformation;
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException("Database error occurred while retrieving DeliveryInformation.", dbEx);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<OrderCancellationHistory>> GetOrderCancellationHistoryByIdAsync(int orderId)
        {
            try
            {
                return await _context.OrderCancellationHistories
                    .Include(och => och.Cashier)
                    .ThenInclude(c => c.UserInformation)
                    .Include(och => och.Order)
                    .Include(och => och.Product)
                    .Where(och => och.OrderId == orderId)
                    .ToListAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Database error occurred while retrieving OrderCancellationHistory.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving OrderCancellationHistory.", ex);
            }
        }

        public async Task<DeliveryInformation?> GetDeliveryInformationByIdAsync(int DeliveryInformationId)
        {
            try
            {
                return await _context.DeliveryInformations
                    .FirstOrDefaultAsync(och => och.DeliveryInformationId == DeliveryInformationId);
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Database error occurred while retrieving DeliveryInformation.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving DeliveryInformation.", ex);
            }
        }

        public async Task<Order?> GetOrderFullInforByIdAsync(int orderId)
        {
            try
            {
                return await _context.Orders.Include(o => o.Customer).FirstOrDefaultAsync(o => o.OrderId == orderId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Pham Son Tung
        public async Task<List<Order>> FilterOrderManagerAsync(FilterOrderManager dto)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderStatus)
                    .Include(o => o.OrderType)
                    .AsQueryable();

                if (dto.OrderId.HasValue)
                {
                    query = query.Where(o => o.OrderId == dto.OrderId.Value);
                }

                if (!string.IsNullOrWhiteSpace(dto.CustomerKeyword))
                {
                    query = query.Where(o =>
                        o.CustomerId.ToString() == dto.CustomerKeyword ||
                        (o.Customer != null && (
                            o.Customer.CustomerName.Contains(dto.CustomerKeyword) ||
                            o.Customer.Phone.Contains(dto.CustomerKeyword)))
                    );
                }

                if (dto.FromDate.HasValue && dto.ToDate.HasValue)
                {
                    query = query.Where(o =>
                        o.CreatedTime >= dto.FromDate.Value &&
                        o.CreatedTime <= dto.ToDate.Value.AddDays(1).AddTicks(-1));
                }

                if (dto.OrderStatus.HasValue)
                {
                    query = query.Where(o => o.OrderStatusId == dto.OrderStatus.Value);
                }

                if (dto.Ordertype.HasValue)
                {
                    query = query.Where(o => o.OrderTypeId == dto.Ordertype.Value);
                }

                query = query
                    .OrderBy(o => o.CreatedTime)
                    .AsNoTracking();

                return await query.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
