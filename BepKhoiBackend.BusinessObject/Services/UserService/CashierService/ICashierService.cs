using BepKhoiBackend.BusinessObject.dtos.UserDto.CashierDto;
using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.UserService.CashierService
{
    public interface ICashierService
    {
        List<CashierDTO> GetAllCashiers();
        GetCashierDTO? GetCashierById(int id);
        void CreateCashier(string email, string password, string phone, string userName);
        Task<bool> UpdateCashier(int userId, string email, string phone, string userName,
                                   string address, string provinceCity, string district,
                                   string wardCommune, DateTime? dateOfBirth);
        void DeleteCashier(int userId);
        List<CashierInvoiceDTO> GetCashierInvoices(int cashierId);
        List<CashierDTO> GetCashiers(string? searchTerm, bool? status);
        byte[] ExportCashiersToExcel();

    }
}
