using BepKhoiBackend.BusinessObject.dtos.UserDto.ShipperDto;
using System.Collections.Generic;

namespace BepKhoiBackend.BusinessObject.Services.UserService.ShipperService
{
    public interface IShipperService
    {
        List<ShipperDTO> GetAllShippers();
        GetShipperDTO? GetShipperById(int id);
        void CreateShipper(string email, string password, string phone, string userName);
        bool UpdateShipper(int userId, string email, string phone, string userName,
                    string address, string provinceCity, string district, string wardCommune,
                    DateTime? dateOfBirth);
        void DeleteShipper(int userId);
        List<ShipperInvoiceDTO> GetShipperInvoices(int shipperId);
        List<ShipperDTO> GetShippers(string searchTerm = null, bool? status = null);
        byte[] ExportShippersToExcel(); // Thêm phương thức xuất file Excel

    }
}
