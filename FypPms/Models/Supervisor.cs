using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FypPms.Models
{
    public partial class Supervisor
    {
        public Supervisor()
        {
            Supervision = new HashSet<Supervision>();
        }

        [DisplayName("Supervisor ID")]
        public int SupervisorId { get; set; }
        [DisplayName("Supervisor ID")]
        public string AssignedId { get; set; }
        [DisplayName("Supervisor Name")]
        public string SupervisorName { get; set; }
        [DisplayName("Supervisor Status")]
        public string SupervisorStatus { get; set; }
        [DisplayName("Supervisor Gender")]
        public string SupervisorGender { get; set; }
        [DisplayName("Supervisor Specialization")]
        public string SupervisorSpecialization { get; set; }
        [DisplayName("Supervisor Phone")]
        public string SupervisorPhone { get; set; }
        [DisplayName("Supervisor Email")]
        public string SupervisorEmail { get; set; }
        [DisplayName("Is Committee Member")]
        public bool IsCommittee { get; set; }
        [DisplayName("User ID")]
        public int? UserId { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }

        public User User { get; set; }
        public ICollection<Supervision> Supervision { get; set; }
    }
}
