using BepKhoiBackend.DataAccess.Models;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Repository.UserRepository.ShipperRepository
{
    public interface IShipperRepository
    {
        List<User> GetAllShippers();
        User? GetShipperById(int id);
        void CreateShipper(string email, string password, string phone, string userName);
        bool UpdateShipper(int userId, string email, string phone, string userName, string address,
                           string provinceCity, string district, string wardCommune, DateTime? dateOfBirth);
        void DeleteShipper(int userId);
        List<Invoice> GetShipperInvoices(int shipperId);
        List<User> GetShippers(string searchTerm = null, bool? status = null);

    }
}
