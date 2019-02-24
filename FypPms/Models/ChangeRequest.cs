using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FypPms.Models
{
    public class ChangeRequest
    {
        public int ChangeRequestId { get; set; }
        [DisplayName("Change Request Type")]
        public string ChangeRequestType { get; set; }
        [DisplayName("Change Request Status")]
        public string ChangeRequestStatus { get; set; }
        [DisplayName("Project Title")]
        public string Title { get; set; }
        [DisplayName("Project Description")]
        public string Description { get; set; }
        [DisplayName("Project Objective")]
        public string Objective { get; set; }
        [DisplayName("Project Scope")]
        public string Scope { get; set; }
        [DisplayName("Reason to Change")]
        public string ReasonToChange { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }

        [DisplayName("Project ID")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
