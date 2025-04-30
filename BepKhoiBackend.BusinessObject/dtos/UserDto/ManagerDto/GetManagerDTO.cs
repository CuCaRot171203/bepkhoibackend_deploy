using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.UserDto.ManagerDto
{
    public class GetManagerDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Province_City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward_Commune { get; set; } = string.Empty;
        public DateTime? Date_of_Birth { get; set; }
    }
}
