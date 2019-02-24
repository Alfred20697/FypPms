using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FypPms.Models
{
    public partial class User
    {
        public User()
        {
            Coordinator = new HashSet<Coordinator>();
            Student = new HashSet<Student>();
            Supervisor = new HashSet<Supervisor>();
            UserRole = new HashSet<UserRole>();
        }

        public User(string userName, string password, string userType)
        {
            UserName = userName;
            UserPassword = password;
            UserType = userType;
            UserStatus = "Active";
        }

        [DisplayName("User ID")]
        public int UserId { get; set; }
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [DisplayName("User Password")]
        [StringLength(16, MinimumLength = 8)]
        public string UserPassword { get; set; }
        [DisplayName("User Type")]
        public string UserType { get; set; }
        [DisplayName("User Status")]
        public string UserStatus { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }

        public ICollection<Coordinator> Coordinator { get; set; }
        public ICollection<Student> Student { get; set; }
        public ICollection<Supervisor> Supervisor { get; set; }
        public ICollection<UserRole> UserRole { get; set; }
    }
}
