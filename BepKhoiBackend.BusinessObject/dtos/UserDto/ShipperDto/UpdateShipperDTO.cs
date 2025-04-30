using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.UserDto.ShipperDto
{
    public class UpdateShipperDTO
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string ProvinceCity { get; set; }
        public string District { get; set; }
        public string WardCommune { get; set; }
        public DateTime? DateOfBirth { get; set; }

    }

}
