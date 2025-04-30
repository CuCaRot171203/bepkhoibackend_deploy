using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Models.ExtendObjects;
using BepKhoiBackend.DataAccess.Repository.InvoiceRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BepKhoiBackend.DataAccess.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly bepkhoiContext _context;

        public InvoiceRepository(bepkhoiContext context)
        {
            _context = context;
        }

        public async Task<List<Invoice>> GetAllInvoices()
        {
            try
            {
                return await _context.Invoices
                    .Include(i => i.InvoiceDetails)
                    .Include(i => i.PaymentMethod)
                    .Include(i => i.OrderType)
                    .Include(i => i.Cashier).ThenInclude(i => i.UserInformation)
                    .Include(i => i.Shipper).ThenInclude(i => i.UserInformation)
                    .Include(i => i.Customer)
                    .Include(i => i.Room)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi truy vấn danh sách hóa đơn.", ex);
            }
        }
        public async Task<List<Invoice>> FilterInvoiceManagerAsync(FilterInvoiceManager dto)
        {
            try
            {
                var query = _context.Invoices
                    .Include(i => i.InvoiceDetails)
                    .Include(i => i.PaymentMethod)
                    .Include(i => i.OrderType)
                    .Include(i => i.Cashier).ThenInclude(c => c.UserInformation)
                    .Include(i => i.Shipper).ThenInclude(s => s.UserInformation)
                    .Include(i => i.Customer)
                    .Include(i => i.Room)
                    .AsQueryable();

                if (dto.InvoiceId.HasValue)
                {
                    query = query.Where(i => i.InvoiceId == dto.InvoiceId.Value);
                }

                if (!string.IsNullOrWhiteSpace(dto.CustomerKeyword))
                {
                    query = query.Where(i =>
                        i.CustomerId.ToString() == dto.CustomerKeyword ||
                        (i.Customer != null && (
                            i.Customer.CustomerName.Contains(dto.CustomerKeyword) ||
                            i.Customer.Phone.Contains(dto.CustomerKeyword)))
                    );
                }

                if (!string.IsNullOrWhiteSpace(dto.CashierKeyword))
                {
                    query = query.Where(i =>
                        i.CashierId.ToString() == dto.CashierKeyword ||
                        (i.Cashier != null &&
                         (i.Cashier.Phone.Contains(dto.CashierKeyword) ||
                         (i.Cashier.UserInformation != null &&
                         i.Cashier.UserInformation.UserName.Contains(dto.CashierKeyword))))
                    );
                }

                if (dto.FromDate.HasValue && dto.ToDate.HasValue)
                {
                    query = query.Where(i =>
                        i.CheckInTime >= dto.FromDate.Value &&
                        i.CheckOutTime <= dto.ToDate.Value);
                }

                if (dto.Status.HasValue)
                {
                    query = query.Where(i => i.Status == dto.Status.Value);
                }

                if (dto.PaymentMethod.HasValue)
                {
                    query = query.Where(i =>
                        i.PaymentMethodId == dto.PaymentMethod.Value);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi truy vấn danh sách hóa đơn", ex);
            }
        }


        //------------------NgocQuan----------------------//
        public Invoice? GetInvoiceForPdf(int id)
        {
            return _context.Invoices
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .Include(i => i.Customer)
                .FirstOrDefault(i => i.InvoiceId == id);
        }
        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            try
            {
                return await _context.Invoices.Include(i => i.Customer).FirstOrDefaultAsync(i => i.InvoiceId == id);
            }
            catch(Exception)
            {
                throw;
            }
        }
        public async Task<bool> UpdateInvoiceStatus(int invoiceId, bool status)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            // Nếu hóa đơn không tồn tại, trả về false
            if (invoice == null)
            {
                return false;
            }

            // Cập nhật trạng thái của hóa đơn
            invoice.Status = status;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            return true;
        }

        //Phạm Sơn Tùng
        public async Task<Invoice> CreateInvoiceForPaymentAsync(Invoice invoice)
        {
            if (invoice == null)
            {
                throw new ArgumentNullException(nameof(invoice), "Invoice object cannot be null.");
            }

            try
            {
                await _context.Invoices.AddAsync(invoice);
                await _context.SaveChangesAsync();
                return invoice;
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException("Database update error while creating invoice.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while creating invoice.", ex);
            }
        }

        //phạm sơn tùng
        public async Task<bool> AddInvoiceDetailForPaymentsAsync(List<InvoiceDetail> invoiceDetails)
        {
            try
            {
                if (invoiceDetails == null || !invoiceDetails.Any())
                    throw new ArgumentException("Empty invoice detail list.");
                await _context.InvoiceDetails.AddRangeAsync(invoiceDetails);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException("Database error occur when saving invoice detail.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error has been occur when saving invoice detail.", ex);
            }
        }
        //Phạm Sơn Tùng
        public async Task ChangeOrderStatusAfterPayment(int orderId)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    throw new InvalidOperationException($"Order with ID {orderId} not found.");
                }
                order.OrderStatusId = 2;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("An error occurred while updating the order status in the database.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the order status.", ex);
            }
        }

        //Pham Son Tung
        public async Task CheckOrderBeforePaymentAsync(Invoice invoiceDto)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == invoiceDto.OrderId);

            if (order == null)
                throw new ArgumentException($"Không tìm thấy đơn hàng.");

            if (order.OrderStatusId != 1)
                throw new InvalidOperationException($"Đơn hàng không hợp lệ để thanh toán.");

            if (order.OrderTypeId != invoiceDto.OrderTypeId ||
                order.CustomerId != invoiceDto.CustomerId ||
                order.ShipperId != invoiceDto.ShipperId ||
                order.RoomId != invoiceDto.RoomId ||
                order.TotalQuantity != invoiceDto.TotalQuantity ||
                order.AmountDue != invoiceDto.Subtotal)
            {
                throw new InvalidOperationException("Thông tin thanh toán không đồng bộ với dữ liệu đơn hàng.");
            }
        }


    }
}
