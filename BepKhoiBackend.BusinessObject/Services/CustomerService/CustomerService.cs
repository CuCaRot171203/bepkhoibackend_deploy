using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.CustomerRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepKhoiBackend.BusinessObject.dtos.CustomerDto;
using OfficeOpenXml;
using BepKhoiBackend.BusinessObject.dtos.CustomerDto;

namespace BepKhoiBackend.BusinessObject.Services.CustomerService
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public List<CustomerDTO> GetAllCustomers()
        {
            var customers = _customerRepository.GetAllCustomers();
            return customers.Select(MapToDTO).ToList();
        }

        public CustomerDTO? GetCustomerById(int customerId)
        {
            var customer = _customerRepository.GetCustomerById(customerId);
            return customer != null ? MapToDTO(customer) : null;
        }

        public List<CustomerDTO> SearchCustomers(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<CustomerDTO>();

            var customers = _customerRepository.SearchCustomers(searchTerm);
            return customers.Select(MapToDTO).ToList();
        }

        private CustomerDTO MapToDTO(Customer customer)
        {
            return new CustomerDTO
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                Phone = customer.Phone,
                TotalAmountSpent = customer.Invoices.Sum(i => i.AmountDue)
            };
        }
        public List<CustomerInvoiceDto> GetInvoicesByCustomerId(int customerId)
        {
            var customer = _customerRepository.GetCustomerById(customerId);
            if (customer == null) return new List<CustomerInvoiceDto>();

            return customer.Invoices.Select(invoice => new CustomerInvoiceDto
            {
                InvoiceId = invoice.InvoiceId,
                CheckInTime = invoice.CheckInTime,
                CheckOutTime = invoice.CheckOutTime,
                CashierId = invoice.CashierId,
                AmountDue = invoice.AmountDue
            }).ToList();
        }
        public byte[] ExportCustomersToExcel()
        {
            var customers = _customerRepository.GetAllCustomers();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Customers");

                // Thêm tiêu đề
                worksheet.Cells[1, 1].Value = "Customer ID";
                worksheet.Cells[1, 2].Value = "Customer Name";
                worksheet.Cells[1, 3].Value = "Phone";
                worksheet.Cells[1, 4].Value = "Total Amount Spent";

                // Đổ dữ liệu
                int row = 2;
                foreach (var customer in customers)
                {
                    worksheet.Cells[row, 1].Value = customer.CustomerId;
                    worksheet.Cells[row, 2].Value = customer.CustomerName;
                    worksheet.Cells[row, 3].Value = customer.Phone;
                    worksheet.Cells[row, 4].Value = customer.Invoices.Sum(i => i.AmountDue);
                    row++;
                }

                return package.GetAsByteArray();
            }
        }

        // Method create customer
        public async Task<Customer> CreateNewCustomerAsync(CreateNewCustomerRequest request)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetCustomerByPhoneAsync(request.Phone);
                // Check exist
                if (existingCustomer != null)
                {
                    throw new InvalidOperationException("Phone number is already registered.");
                }

                var newCustomer = new Customer
                {
                    Phone = request.Phone,
                    CustomerName = request.CustomerName,
                    IsDelete = false,
                };

                await _customerRepository.CreateCustomerAsync(newCustomer);
                return newCustomer;
            }
            catch
            {
                throw;
            }
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            try
            {
                // Gọi repository để xóa khách hàng
                await _customerRepository.DeleteCustomerAsync(customerId);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException($"Error while deleting customer: {ex.Message}", ex);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateCustomerAsync(int customerId, string phone, string customerName)
        {
            try
            {
                await _customerRepository.UpdateCustomerAsync(customerId, phone, customerName);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException($"Error while updating customer: {ex.Message}", ex);
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
