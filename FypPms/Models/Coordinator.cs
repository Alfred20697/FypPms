using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FypPms.Models
{
    public class Coordinator
    {
        [DisplayName("Coordinator ID")]
        public int CoordinatorId { get; set; }
        [DisplayName("Coordinator ID")]
        public string AssignedId { get; set; }
        [DisplayName("Coordinator Name")]
        public string CoordinatorName { get; set; }
        [DisplayName("Coordinator Status")]
        public string CoordinatorStatus { get; set; }
        [DisplayName("Coordinator Gender")]
        public string CoordinatorGender { get; set; }
        [DisplayName("Coordinator Phone")]
        public string CoordinatorPhone { get; set; }
        [DisplayName("Coordinator Email")]
        public string CoordinatorEmail { get; set; }
        [DisplayName("User ID")]
        public int? UserId { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }

        public User User { get; set; }
    }
}
