using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.UserRepository.CashierRepository
{
    public interface ICashierRepository
    {
        List<User> GetAllCashiers();
        User? GetCashierById(int id);
        void CreateCashier(string email, string password, string phone, string userName);
        Task<bool> UpdateCashier(int userId, string email, string phone, string userName, string address,
                                          string provinceCity, string district, string wardCommune, DateTime? dateOfBirth);
        void DeleteCashier(int userId);
        List<Invoice> GetCashierInvoices(int cashierId);
        List<User> GetCashiers(string? searchTerm, bool? status);


    }
}
