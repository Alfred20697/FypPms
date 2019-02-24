using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FypPms.Models
{
    public partial class UserRole
    {
        [DisplayName("User ID")]
        public int UserId { get; set; }
        [DisplayName("Role ID")]
        public int RoleId { get; set; }

        public Role Role { get; set; }
        public User User { get; set; }
    }
}
