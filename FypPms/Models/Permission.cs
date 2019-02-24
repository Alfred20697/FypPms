using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FypPms.Models
{
    public partial class Permission
    {
        public Permission()
        {
            RolePermission = new HashSet<RolePermission>();
        }

        [DisplayName("Permission ID")]
        public int PermissionId { get; set; }
        [DisplayName("Permission Name")]
        public string PermissionName { get; set; }
        [DisplayName("Permission Description")]
        public string PermissionDescription { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }

        public ICollection<RolePermission> RolePermission { get; set; }
    }
}
