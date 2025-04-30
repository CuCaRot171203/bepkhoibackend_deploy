using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepKhoiBackend.BusinessObject.dtos.CustomerDto;
using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.BusinessObject.Services.CustomerService
{
    public interface ICustomerService
    {
        List<CustomerDTO> GetAllCustomers();
        CustomerDTO? GetCustomerById(int customerId);
        List<CustomerDTO> SearchCustomers(string searchTerm);
        List<CustomerInvoiceDto> GetInvoicesByCustomerId(int customerId);
        byte[] ExportCustomersToExcel();
        Task<Customer> CreateNewCustomerAsync(CreateNewCustomerRequest request);
        Task DeleteCustomerAsync(int customerId);
        Task UpdateCustomerAsync(int customerId, string phone, string customerName);


    }
}
