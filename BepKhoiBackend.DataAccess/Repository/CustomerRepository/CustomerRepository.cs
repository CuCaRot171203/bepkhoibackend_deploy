using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BepKhoiBackend.DataAccess.Repository.CustomerRepository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly bepkhoiContext _context;

        public CustomerRepository(bepkhoiContext context)
        {
            _context = context;
        }

        public List<Customer> GetAllCustomers()
        {
            return _context.Customers
                .Include(c => c.Invoices)
                .Where(c => c.IsDelete != true)
                .ToList();
        }

        public Customer? GetCustomerById(int customerId)
        {
            return _context.Customers
                .Include(c => c.Invoices)
                .FirstOrDefault(c => c.CustomerId == customerId && (c.IsDelete == false || c.IsDelete == null));
        }

        public List<Customer> SearchCustomers(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return new List<Customer>();
            keyword = keyword.Trim();

            return _context.Customers
                .Include(c => c.Invoices)
                .Where(c => (c.CustomerName.Contains(keyword) || c.Phone.Contains(keyword))
                            && (c.IsDelete == false || c.IsDelete == null))
                .ToList();
        }

        // ====== Customer Repo - Thanh Tung ======

        // Func to get c by phone number
        public async Task<Customer?> GetCustomerByPhoneAsync(string phone)
        {
            return await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Phone == phone && c.IsDelete != true);
        }

        //Phạm Sơn Tùng
        public async Task CreateCustomerAsync(Customer customer)
        {
            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Phạm Sơn Tùng
        public async Task DeleteCustomerAsync(int customerId)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(customerId);

                if (customer == null)
                {
                    throw new ArgumentException($"Customer with ID {customerId} not found.");
                }

                customer.IsDelete = true;
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
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
                var customer = await _context.Customers.FindAsync(customerId);
                var existPhone = await _context.Customers.FirstOrDefaultAsync(c => c.Phone == phone && c.CustomerId != customerId);
                if (customer == null)
                {
                    throw new ArgumentException($"Customer with ID {customerId} not found.");
                }
                if (existPhone != null)
                {
                    throw new ArgumentException($"Exist phone number.");
                }
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw; 
            }
        }




    }
}
