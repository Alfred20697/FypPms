using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FypPms.Models
{
    public partial class Role
    {
        public Role()
        {
            RolePermission = new HashSet<RolePermission>();
            UserRole = new HashSet<UserRole>();
        }

        [DisplayName("Role ID")]
        public int RoleId { get; set; }
        [DisplayName("Role Name")]
        public string RoleName { get; set; }
        [DisplayName("Role Description")]
        public string RoleDescription { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }

        public ICollection<RolePermission> RolePermission { get; set; }
        public ICollection<UserRole> UserRole { get; set; }
    }
}
