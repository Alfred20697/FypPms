using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FypPms.Models
{
    public class Submission
    {
        public int SubmissionId { get; set; }
        [DisplayName("Submission Name")]
        public string SubmissionName { get; set; }
        [DisplayName("Submission Folder")]
        public string SubmissionFolder { get; set; }
        [DisplayName("Submission File")]
        public string SubmissionFile { get; set; }
        [DisplayName("Submission Size")]
        public long SubmissionSize { get; set; }
        [DisplayName("Date Uploaded")]
        public DateTime UploadDate { get; set; }
        [DisplayName("Submission Status")]
        public string SubmissionStatus { get; set; }
        [DisplayName("Project ID")]
        public int ProjectId { get; set; }
        [DisplayName("Submission Type")]
        public int SubmissionTypeId { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }

        public Project Project { get; set; }
    }
}
