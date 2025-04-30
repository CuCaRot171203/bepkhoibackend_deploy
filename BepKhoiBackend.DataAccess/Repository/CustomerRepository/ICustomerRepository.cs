using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.CustomerRepository
{
    public interface ICustomerRepository
    {
        List<Customer> GetAllCustomers();
        Customer? GetCustomerById(int customerId);
        List<Customer> SearchCustomers(string keyword);
        Task<Customer?> GetCustomerByPhoneAsync(string phone);
        Task CreateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int customerId);
        Task UpdateCustomerAsync(int customerId, string phone, string customerName);
    }
}
