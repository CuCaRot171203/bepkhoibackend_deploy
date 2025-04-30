using BepKhoiBackend.BusinessObject.dtos.UserDto.ShipperDto;
using BepKhoiBackend.DataAccess.Repository.UserRepository.ShipperRepository;
using OfficeOpenXml;
using System.Collections.Generic;

namespace BepKhoiBackend.BusinessObject.Services.UserService.ShipperService
{
    public class ShipperService : IShipperService
    {
        private readonly IShipperRepository _shipperRepository;

        public ShipperService(IShipperRepository shipperRepository)
        {
            _shipperRepository = shipperRepository;
        }

        public List<ShipperDTO> GetAllShippers()
        {
            var shippers = _shipperRepository.GetAllShippers();
            return shippers
                .Where(s => s.UserInformation != null) // Lọc ra các User có thông tin hợp lệ
                .Select(s => new ShipperDTO
                {
                    UserId = s.UserId,
                    UserName = s.UserInformation?.UserName ?? "Unknown", // Kiểm tra null
                    Phone = s.Phone,
                    Status = s.Status
                }).ToList();
        }

        public GetShipperDTO GetShipperById(int id)
        {
            var shipper = _shipperRepository.GetShipperById(id);
            if (shipper == null || shipper.UserInformation == null) return null; // Kiểm tra null

            return new GetShipperDTO
            {
                UserId = shipper.UserId,
                UserName = shipper.UserInformation?.UserName ?? "Unknown",
                RoleName = shipper.Role?.RoleName ?? "Unknown",
                Phone = shipper.Phone ?? "N/A",
                Email = shipper.Email ?? "N/A",
                Address = shipper.UserInformation?.Address ?? "N/A",
                Province_City = shipper.UserInformation?.ProvinceCity ?? "N/A",
                District = shipper.UserInformation?.District ?? "N/A",
                Ward_Commune = shipper.UserInformation?.WardCommune ?? "N/A",
                Date_of_Birth = shipper.UserInformation?.DateOfBirth ?? null,
            };
        }


        public void CreateShipper(string email, string password, string phone, string userName)
        {
            _shipperRepository.CreateShipper(email, password, phone, userName);
        }

        public bool UpdateShipper(int userId, string email, string phone, string userName,
                          string address, string provinceCity, string district, string wardCommune,
                          DateTime? dateOfBirth)
        {
            try
            {
                return _shipperRepository.UpdateShipper(userId, email, phone, userName,
                                        address, provinceCity, district, wardCommune, dateOfBirth);
            }
            catch (Exception)
            {
                throw;
            }
        }



        public void DeleteShipper(int userId)
        {
            _shipperRepository.DeleteShipper(userId);
        }
        public List<ShipperInvoiceDTO> GetShipperInvoices(int shipperId)
        {
            var invoices = _shipperRepository.GetShipperInvoices(shipperId);

            return invoices.Select(i => new ShipperInvoiceDTO
            {
                InvoiceId = i.InvoiceId,
                CustomerId = i.CustomerId ?? 0,
                CustomerName = i.Customer != null ? i.Customer.CustomerName : "Unknown",
                CheckInTime = i.CheckInTime,
                AmountDue = i.AmountDue,
                Status = i.Status,
                PaymentMethodId = i.PaymentMethodId,
                PaymentMethodName = i.PaymentMethod?.PaymentMethodTitle ?? "Unknown", // Lấy tên phương thức thanh toán
                InvoiceDetails = i.InvoiceDetails.Select(d => new ShipperInvoiceDetailDTO
                {
                    InvoiceDetailId = d.InvoiceDetailId,
                    ProductId = d.ProductId,
                    ProductName = d.Product?.ProductName ?? "Unknown",
                    Quantity = d.Quantity,
                    ProductPrice = d.Price,
                    ProductVAT = d.ProductVat ?? 0m,
                    ProductNote = d.ProductNote
                }).ToList()
            }).ToList();
        }
        public List<ShipperDTO> GetShippers(string searchTerm = null, bool? status = null)
        {
            var shippers = _shipperRepository.GetShippers(searchTerm, status);
            return shippers
                .Where(s => s.UserInformation != null)
                .Select(s => new ShipperDTO
                {
                    UserId = s.UserId,
                    UserName = s.UserInformation?.UserName ?? "Unknown",
                    Phone = s.Phone,
                    Status = s.Status
                }).ToList();
        }
        public byte[] ExportShippersToExcel()
        {
            var shippers = _shipperRepository.GetAllShippers();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Shippers");

                // Thêm tiêu đề
                worksheet.Cells[1, 1].Value = "Shipper ID";
                worksheet.Cells[1, 2].Value = "Shipper Name";
                worksheet.Cells[1, 3].Value = "Phone";
                worksheet.Cells[1, 4].Value = "Total Orders";

                // Đổ dữ liệu
                int row = 2;
                foreach (var shipper in shippers)
                {

                    worksheet.Cells[row, 1].Value = shipper.UserId;
                    worksheet.Cells[row, 2].Value = shipper.UserInformation.UserName;
                    worksheet.Cells[row, 3].Value = shipper.Phone;
                    worksheet.Cells[row, 4].Value = shipper.Orders.Count;
                    row++;
                }

                return package.GetAsByteArray();
            }
        }

        }
}
