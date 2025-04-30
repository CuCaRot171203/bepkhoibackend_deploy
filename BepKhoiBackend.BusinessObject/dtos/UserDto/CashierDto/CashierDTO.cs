using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.UserDto.CashierDto
{
    public class CashierDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool? Status { get; set; }
    }
}
