using System;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Models
{
    public partial class UserRole
    {
        public UserRole()
        {
            Users = new HashSet<User>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public bool? IsDelete { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
