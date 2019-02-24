using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FypPms.Models
{
    public partial class RolePermission
    {
        [DisplayName("Role ID")]
        public int RoleId { get; set; }
        [DisplayName("Permission ID")]
        public int PermissionId { get; set; }

        public Permission Permission { get; set; }
        public Role Role { get; set; }
    }
}
