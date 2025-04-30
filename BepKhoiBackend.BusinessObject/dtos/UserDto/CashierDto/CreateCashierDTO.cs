using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.UserDto.CashierDto
{
    public class CreateCashierDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
    }
}
