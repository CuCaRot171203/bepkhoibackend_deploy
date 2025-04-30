using BepKhoiBackend.BusinessObject.dtos.UserDto.CashierDto;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.UserRepository.CashierRepository;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.UserService.CashierService
{
    public class CashierService : ICashierService
    {
        private readonly ICashierRepository _cashierRepository;

        public CashierService(ICashierRepository cashierRepository)
        {
            _cashierRepository = cashierRepository;
        }

        public List<CashierDTO> GetAllCashiers()
        {
            var cashiers = _cashierRepository.GetAllCashiers();
            return cashiers
                .Where(c => c.UserInformation != null) // Lọc các Cashier có UserInformation hợp lệ
                .Select(c => new CashierDTO
                {
                    UserId = c.UserId,
                    UserName = c.UserInformation?.UserName ?? "Unknown", // Kiểm tra null
                    Phone = c.Phone,
                    Status = c.Status
                }).ToList();
        }

        public GetCashierDTO GetCashierById(int id)
        {
            var cashier = _cashierRepository.GetCashierById(id);
            if (cashier == null || cashier.UserInformation == null) return null; // Kiểm tra null

            return new GetCashierDTO
            {
                UserId = cashier.UserId,
                UserName = cashier.UserInformation?.UserName ?? "Unknown", // Kiểm tra null
                RoleName = cashier.Role?.RoleName ?? "Unknown",
                Phone = cashier.Phone ?? "N/A",
                Email = cashier.Email ?? "N/A",
                Address = cashier.UserInformation?.Address ?? "N/A",
                Province_City = cashier.UserInformation?.ProvinceCity ?? "N/A",
                District = cashier.UserInformation?.District ?? "N/A",
                Ward_Commune = cashier.UserInformation?.WardCommune ?? "N/A",
                Date_of_Birth = cashier.UserInformation?.DateOfBirth ?? null,
            };
        }


        public void CreateCashier(string email, string password, string phone, string userName)
        {
            _cashierRepository.CreateCashier(email, password, phone, userName);
        }

        public async Task<bool> UpdateCashier(int userId, string email, string phone, string userName,
                           string address, string provinceCity, string district,
                           string wardCommune, DateTime? dateOfBirth)
        {
            try
            {
                return await  _cashierRepository.UpdateCashier(userId, email, phone, userName,
                                        address, provinceCity, district,
                                        wardCommune, dateOfBirth);
            }
            catch (Exception) {
                throw;
            }
        }


        public void DeleteCashier(int userId)
        {
            _cashierRepository.DeleteCashier(userId);
        }

        public List<CashierInvoiceDTO> GetCashierInvoices(int cashierId)
        {
            var invoices = _cashierRepository.GetCashierInvoices(cashierId);

            return invoices.Select(i => new CashierInvoiceDTO
            {
                InvoiceId = i.InvoiceId,
                CustomerId = i.CustomerId ?? 0,
                CustomerName = i.Customer != null ? i.Customer.CustomerName : "Unknown",
                CheckInTime = i.CheckInTime,
                AmountDue = i.AmountDue,
                Status = i.Status,
                PaymentMethodId = i.PaymentMethodId,
                PaymentMethodName = i.PaymentMethod?.PaymentMethodTitle ?? "Unknown",
                InvoiceDetails = i.InvoiceDetails.Select(d => new CashierInvoiceDetailDTO
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

        public List<CashierDTO> GetCashiers(string? searchTerm, bool? status)
        {
            var cashiers = _cashierRepository.GetCashiers(searchTerm, status);

            return cashiers
                .Where(c => c.UserInformation != null)
                .Select(c => new CashierDTO
                {
                    UserId = c.UserId,
                    UserName = c.UserInformation?.UserName ?? "Unknown",
                    Phone = c.Phone,
                    Status = c.Status
                }).ToList();
        }
        public byte[] ExportCashiersToExcel()
        {
            var cashiers = _cashierRepository.GetAllCashiers();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Cashiers");

                // Thêm tiêu đề
                worksheet.Cells[1, 1].Value = "Cashier ID";
                worksheet.Cells[1, 2].Value = "Cashier Name";
                worksheet.Cells[1, 3].Value = "Phone";
                worksheet.Cells[1, 4].Value = "Total Invoices";

                // Đổ dữ liệu
                int row = 2;
                foreach (var cashier in cashiers)
                {
                    worksheet.Cells[row, 1].Value = cashier.UserId;
                    worksheet.Cells[row, 2].Value = cashier.UserInformation.UserName;
                    worksheet.Cells[row, 3].Value = cashier.Phone;
                    worksheet.Cells[row, 4].Value = cashier.InvoiceCashiers.Count; // Tổng số đơn hàng mà Cashier xử lý
                    row++;
                }

                return package.GetAsByteArray();
            }
        }

    }
}
