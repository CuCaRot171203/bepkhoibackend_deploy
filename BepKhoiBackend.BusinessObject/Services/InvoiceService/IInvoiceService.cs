using BepKhoiBackend.BusinessObject.dtos.InvoiceDto;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Models.ExtendObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.InvoiceService
{
    public interface IInvoiceService
    {
        Task<List<InvoiceDTO>> GetAllInvoicesAsync();
        Task<List<InvoiceDTO>> FilterInvoiceManagerServiceAsync(FilterInvoiceManager dto);
        //------------------NgocQuan----------------------//
        InvoicePdfDTO GetInvoiceForPdf(int id);
        Task<InvoiceForVnpayProcessDto?> GetInvoiceByIdForVnpayAsync(int invoiceId);
        Task<bool> UpdateInvoiceStatus(int invoiceId, bool status);
        Task<(int invoiceId, int? roomId, bool? isUse)> CreateInvoiceForPaymentServiceAsync(
            InvoiceForPaymentDto invoiceDto,
            List<InvoiceDetailForPaymentDto> detailDtos);
        Task<(Invoice invoice, (int? roomId, bool? isUse)? roomUpdateResult)> HandleInvoiceVnpayCompletionAsync(int invoiceId);
    }

}
