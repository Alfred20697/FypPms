using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FypPms.Models
{
    public partial class Review
    {
        [DisplayName("Review ID")]
        public int ReviewId { get; set; }
        [DisplayName("Reviewer")]
        public string Reviewer { get; set; }
        [DisplayName("Review Comment")]
        public string ReviewComment { get; set; }
        [DisplayName("Review Status")]
        public string ReviewStatus { get; set; }
        [DisplayName("Project ID")]
        public int ProjectId { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }

        public Project Project { get; set; }
    }
}
