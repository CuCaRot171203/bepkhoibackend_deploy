using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class UserInformation
    {
        public UserInformation()
        {
            Users = new HashSet<User>();
        }

        public int UserInformationId { get; set; }
        public string UserName { get; set; } = null!;
        public string? Address { get; set; }
        public string? ProvinceCity { get; set; }
        public string? District { get; set; }
        public string? WardCommune { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
