using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FypPms.Models
{
    public class SubmissionType
    {
        public int SubmissionTypeId { get; set; }
        [DisplayName("Submission Type")]
        public string Name { get; set; }
        [DisplayName("Submission Start Date")]
        public DateTime StartDate { get; set; }
        [DisplayName("Submission End Date")]
        public DateTime EndDate { get; set; }
        [DisplayName("Submission Grace Period")]
        public DateTime GraceDate { get; set; }
        [DisplayName("Submission Status")]
        public string Status { get; set; }
        [DisplayName("Batch")]
        public int? BatchId { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }

        public Batch Batch { get; set; }
    }
}
