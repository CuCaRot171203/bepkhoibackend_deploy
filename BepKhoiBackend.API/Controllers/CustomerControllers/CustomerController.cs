using BepKhoiBackend.BusinessObject.dtos.CustomerDto;
using BepKhoiBackend.BusinessObject.Services.CustomerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BepKhoiBackend.API.Controllers.CustomerControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [Authorize(Roles = "manager, cashier")]
        [HttpGet]
        public ActionResult<List<CustomerDTO>> GetAllCustomers()
        {
            try
            {
                var customers = _customerService.GetAllCustomers();
                return Ok(customers);
            }catch(Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Roles = "manager, cashier")]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerDTO), 200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(404)] // NotFound
        [ProducesResponseType(500)] // Internal Server Error
        public ActionResult<CustomerDTO> GetCustomerById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Id phải là số nguyên dương lớn hơn 0." });
                }
                var customer = _customerService.GetCustomerById(id);
                if (customer == null)
                {
                    return NotFound(new { message = "Khách hàng không tồn tại!" });
                }
                return Ok(customer);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(409, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(List<CustomerDTO>), 200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(500)] // Internal Server Error
        public ActionResult<List<CustomerDTO>> SearchCustomersByNameOrPhone([FromQuery] string searchTerm)
        {
            try
            {
                // Kiểm tra điều kiện đầu vào
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    throw new ValidationException("Từ khóa tìm kiếm không được để trống.");
                }

                var customers = _customerService.SearchCustomers(searchTerm);
                return Ok(customers);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server", error = ex.Message });
            }
        }

        [Authorize(Roles = "manager, cashier")]
        [HttpGet("{customerId}/invoices")]
        public IActionResult GetInvoicesByCustomerId(int customerId)
        {
            var invoices = _customerService.GetInvoicesByCustomerId(customerId);
            if (invoices == null || invoices.Count == 0)
            {
                return NotFound("Không tìm thấy hóa đơn nào cho khách hàng này.");
            }
            return Ok(invoices);
        }

        [Authorize(Roles = "manager, cashier")]
        [HttpGet("export")]
        public IActionResult ExportCustomers()
        {
            var fileContents = _customerService.ExportCustomersToExcel();
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customers.xlsx");
        }

        [HttpPost("create-new-customer")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(409)] // Conflict
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> CreateNewCustomerPos([FromBody] CreateNewCustomerRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Phone) || string.IsNullOrWhiteSpace(request.CustomerName))
                {
                    return BadRequest(new { message = "Phone and Customer Name cannot be empty." });
                }

                var result = await _customerService.CreateNewCustomerAsync(request);
                return Ok(new { message = "Customer created successfully", data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        [Authorize(Roles = "manager, cashier")]
        [HttpDelete("delete/{customerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCustomer(int customerId)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(customerId);
                return Ok(new { message = "Customer deleted successfully." });
            }
            catch (InvalidOperationException)
            {
                return BadRequest(new { message = "Customer not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Roles = "manager, cashier")]
        [HttpPut("update/{customerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCustomer(int customerId, [FromQuery] string phone, [FromQuery] string customerName)
        {
            try
            {
                await _customerService.UpdateCustomerAsync(customerId, phone, customerName);
                return Ok(new { message = "Customer updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = "Customer not found or exist phone number."});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }



    }
}
