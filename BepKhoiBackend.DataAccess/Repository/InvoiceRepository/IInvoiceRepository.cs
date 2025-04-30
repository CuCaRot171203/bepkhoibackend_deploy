using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Models.ExtendObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.InvoiceRepository
{
    public interface IInvoiceRepository
    {
        Task<List<Invoice>> GetAllInvoices();
        Task<List<Invoice>> FilterInvoiceManagerAsync(FilterInvoiceManager dto);
        //------------------NgocQuan----------------------//
        Invoice? GetInvoiceForPdf(int id);
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task<bool> UpdateInvoiceStatus(int invoiceId, bool status);
        Task<Invoice> CreateInvoiceForPaymentAsync(Invoice invoice);
        Task<bool> AddInvoiceDetailForPaymentsAsync(List<InvoiceDetail> invoiceDetails);
        Task ChangeOrderStatusAfterPayment(int orderId);
        Task CheckOrderBeforePaymentAsync(Invoice invoiceDto);
    }
}
