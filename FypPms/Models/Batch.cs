using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FypPms.Models
{
    public partial class Batch
    {
        public Batch()
        {
            Student = new HashSet<Student>();
            SubmissionType = new HashSet<SubmissionType>();
        }

        [DisplayName("Batch ID")]
        public int BatchId { get; set; }
        [DisplayName("Batch Name")]
        public string BatchName { get; set; }
        [DisplayName("Batch Start Date")]
        public DateTime BatchStartDate { get; set; }
        [DisplayName("Batch End Date")]
        public DateTime BatchEndDate { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }

        public ICollection<Student> Student { get; set; }
        public ICollection<SubmissionType> SubmissionType { get; set; }
    }
}
