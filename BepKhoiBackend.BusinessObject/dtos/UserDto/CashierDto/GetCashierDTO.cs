using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.UserDto.CashierDto
{
    public class GetCashierDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Province_City { get; set; }
        public string District { get; set; }
        public string Ward_Commune { get; set; }
        public DateTime? Date_of_Birth { get; set; }
    }

}
