using BepKhoiBackend.BusinessObject.dtos.InvoiceDto;
using BepKhoiBackend.DataAccess.Abstract.OrderAbstract;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Models.ExtendObjects;
using BepKhoiBackend.DataAccess.Repository.InvoiceRepository;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BepKhoiBackend.BusinessObject.Services.InvoiceService
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IOrderRepository _orderRepository;
        public InvoiceService(IInvoiceRepository invoiceRepository, IOrderRepository orderRepository)
        {
            _invoiceRepository = invoiceRepository;
            _orderRepository = orderRepository;
        }

        public async Task<List<InvoiceDTO>> GetAllInvoicesAsync()
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllInvoices(); 

                return invoices.Select(i => new InvoiceDTO
                {
                    InvoiceId = i.InvoiceId,
                    PaymentMethod = i.PaymentMethod?.PaymentMethodTitle,
                    OrderId = i.OrderId,
                    OrderType = i.OrderType?.OrderTypeTitle,
                    Cashier = i.Cashier?.UserInformation?.UserName,
                    Shipper = i.Shipper?.UserInformation?.UserName,
                    Customer = i.Customer != null ? $"{i.Customer.CustomerName}-{i.Customer.Phone}" : null,
                    Room = i.Room != null ? $"ID: {i.Room.RoomId} - {i.Room.RoomName}" : null,
                    CheckInTime = i.CheckInTime,
                    CheckOutTime = i.CheckOutTime,
                    TotalQuantity = i.TotalQuantity,
                    Subtotal = i.Subtotal,
                    OtherPayment = i.OtherPayment,
                    InvoiceDiscount = i.InvoiceDiscount,
                    TotalVat = i.TotalVat,
                    AmountDue = i.AmountDue,
                    Status = i.Status,
                    InvoiceNote = i.InvoiceNote,
                    InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetailDTO
                    {
                        InvoiceDetailId = d.InvoiceDetailId,
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        Quantity = d.Quantity,
                        Price = d.Price,
                        ProductVat = d.ProductVat,
                        ProductNote = d.ProductNote
                    }).ToList()
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<InvoiceDTO>> FilterInvoiceManagerServiceAsync(FilterInvoiceManager dto)
        {
            try
            {
                var invoices = await _invoiceRepository.FilterInvoiceManagerAsync(dto);

                return invoices.Select(i => new InvoiceDTO
                {
                    InvoiceId = i.InvoiceId,
                    PaymentMethod = i.PaymentMethod?.PaymentMethodTitle,
                    OrderId = i.OrderId,
                    OrderType = i.OrderType?.OrderTypeTitle,
                    Cashier = i.Cashier?.UserInformation?.UserName,
                    Shipper = i.Shipper?.UserInformation?.UserName,
                    Customer = i.Customer != null ? $"{i.Customer.CustomerName}-{i.Customer.Phone}" : null,
                    Room = i.Room != null ? $"ID: {i.Room.RoomId} - {i.Room.RoomName}" : null,
                    CheckInTime = i.CheckInTime,
                    CheckOutTime = i.CheckOutTime,
                    TotalQuantity = i.TotalQuantity,
                    Subtotal = i.Subtotal,
                    OtherPayment = i.OtherPayment,
                    InvoiceDiscount = i.InvoiceDiscount,
                    TotalVat = i.TotalVat,
                    AmountDue = i.AmountDue,
                    Status = i.Status,
                    InvoiceNote = i.InvoiceNote,
                    InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetailDTO
                    {
                        InvoiceDetailId = d.InvoiceDetailId,
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        Quantity = d.Quantity,
                        Price = d.Price,
                        ProductVat = d.ProductVat,
                        ProductNote = d.ProductNote
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xử lý dữ liệu hóa đơn đã lọc", ex);
            }
        }

        //------------------NgocQuan----------------------//
        public InvoicePdfDTO GetInvoiceForPdf(int id)
        {
            var invoice = _invoiceRepository.GetInvoiceForPdf(id);
            if (invoice == null) return null;

            return new InvoicePdfDTO
            {
                InvoiceId = invoice.InvoiceId,
                CheckInTime = invoice.CheckInTime,
                CheckOutTime = invoice.CheckOutTime,
                CustomerName = invoice.Customer?.CustomerName ?? "Khách lẻ",
                TotalQuantity = invoice.TotalQuantity,
                Subtotal = invoice.Subtotal,
                TotalVat = invoice.TotalVat ?? 0,
                AmountDue = invoice.AmountDue,
                InvoiceDetails = invoice.InvoiceDetails.Select(d => new InvoiceDetailPdfDTO
                {
                    ProductName = d.Product != null ? d.Product.ProductName : "Không xác định",
                    Quantity = d.Quantity,
                    Price = d.Price
                }).ToList()
            };
        }


        public async Task<InvoiceForVnpayProcessDto?> GetInvoiceByIdForVnpayAsync(int invoiceId)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);

                if (invoice == null)
                {
                    return null;
                }
                return new InvoiceForVnpayProcessDto
                {
                    InvoiceId = invoice.InvoiceId,
                    PaymentMethodId = invoice.PaymentMethodId,
                    OrderId = invoice.OrderId,
                    OrderTypeId = invoice.OrderTypeId,
                    CashierId = invoice.CashierId,
                    ShipperId = invoice.ShipperId,
                    CustomerName = invoice.Customer?.CustomerName,
                    RoomId = invoice.RoomId,
                    CheckInTime = invoice.CheckInTime,
                    CheckOutTime = invoice.CheckOutTime,
                    TotalQuantity = invoice.TotalQuantity,
                    Subtotal = invoice.Subtotal,
                    OtherPayment = invoice.OtherPayment,
                    InvoiceDiscount = invoice.InvoiceDiscount,
                    TotalVat = invoice.TotalVat,
                    AmountDue = invoice.AmountDue,
                    Status = invoice.Status,
                    InvoiceNote = invoice.InvoiceNote
                };
            }
            catch (Exception)
            {
                throw; 
            }
        }

        public async Task<bool> UpdateInvoiceStatus(int invoiceId, bool status)
        {
            // Gọi phương thức cập nhật trạng thái hóa đơn trong repository
            bool isUpdated = await _invoiceRepository.UpdateInvoiceStatus(invoiceId, status);

            // Kiểm tra nếu không thành công (ví dụ: không tìm thấy hóa đơn)
            if (!isUpdated)
            {
                // Thực hiện xử lý khi không thể cập nhật trạng thái hóa đơn
                return false;  // Trả về false nếu không thành công
            }

            // Trả về true nếu việc cập nhật thành công
            return true;
        }

        //Pham Son Tung
        public async Task<(Invoice invoice, (int? roomId, bool? isUse)? roomUpdateResult)> HandleInvoiceVnpayCompletionAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId); 
            if (invoice == null)
                throw new ArgumentException("Invoice not found with ID: " + invoiceId);

            await _invoiceRepository.ChangeOrderStatusAfterPayment(invoice.OrderId); 

            (int? roomId, bool? isUse)? roomUpdateResult = null;

            if (invoice.OrderTypeId == 3 && invoice.RoomId.HasValue)
            {
                var result = await _orderRepository.UpdateRoomIsUseByRoomIdAsync(invoice.RoomId.Value);
                roomUpdateResult = (result.roomId, result.isUse);
            }

            return (invoice, roomUpdateResult);
        }

        //Phạm sơn tùng
        public async Task<(int invoiceId, int? roomId, bool? isUse)> CreateInvoiceForPaymentServiceAsync(
            InvoiceForPaymentDto invoiceDto,
            List<InvoiceDetailForPaymentDto> detailDtos)
        {
            if (invoiceDto == null) throw new ArgumentNullException(nameof(invoiceDto));
            if (detailDtos == null || !detailDtos.Any()) throw new ArgumentException("Chi tiết hóa đơn không được để trống.");

            try
            {
                var invoice = new Invoice
                {
                    PaymentMethodId = invoiceDto.PaymentMethodId,
                    OrderId = invoiceDto.OrderId,
                    OrderTypeId = invoiceDto.OrderTypeId,
                    CashierId = invoiceDto.CashierId,
                    ShipperId = invoiceDto.ShipperId,
                    CustomerId = invoiceDto.CustomerId,
                    RoomId = invoiceDto.RoomId,
                    CheckInTime = invoiceDto.CheckInTime,
                    CheckOutTime = invoiceDto.CheckOutTime,
                    TotalQuantity = invoiceDto.TotalQuantity,
                    Subtotal = invoiceDto.Subtotal,
                    OtherPayment = invoiceDto.OtherPayment,
                    InvoiceDiscount = invoiceDto.InvoiceDiscount,
                    TotalVat = invoiceDto.TotalVat,
                    AmountDue = invoiceDto.AmountDue,
                    Status = invoiceDto.Status ?? false,
                    InvoiceNote = invoiceDto.InvoiceNote
                };
                await _invoiceRepository.CheckOrderBeforePaymentAsync(invoice);
                var createdInvoice = await _invoiceRepository.CreateInvoiceForPaymentAsync(invoice);

                var invoiceDetails = detailDtos.Select(d => new InvoiceDetail
                {
                    InvoiceId = createdInvoice.InvoiceId,
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    ProductVat = d.ProductVat,
                    ProductNote = d.ProductNote
                }).ToList();

                await _invoiceRepository.AddInvoiceDetailForPaymentsAsync(invoiceDetails);
                if (invoice.Status == true)
                {
                    await _invoiceRepository.ChangeOrderStatusAfterPayment(invoiceDto.OrderId);
                    // Nếu là đơn tại bàn thì cập nhật trạng thái phòng
                    int? updatedRoomId = null;
                    bool? isUse = null;
                    if (invoiceDto.OrderTypeId == 3 && invoiceDto.RoomId.HasValue)
                    {
                        var result = await _orderRepository.UpdateRoomIsUseByRoomIdAsync(invoiceDto.RoomId.Value);
                        updatedRoomId = result.roomId;
                        isUse = result.isUse;
                    }
                    return (createdInvoice.InvoiceId, updatedRoomId, isUse);
                }
                else
                {
                    return (createdInvoice.InvoiceId, null, null);
                }
            }
            catch (DbUpdateException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
