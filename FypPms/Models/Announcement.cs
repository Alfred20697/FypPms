using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FypPms.Models
{
    public class Announcement
    {
        [DisplayName("Announcement ID")]
        public int AnnouncementId { get; set; }
        [DisplayName("Announcement Subject")]
        public string AnnouncementSubject { get; set; }
        [DisplayName("Announcement Body")]
        public string AnnouncementBody { get; set; }
        [DisplayName("Announcement Status")]
        public string AnnouncementStatus { get; set; }
        [DisplayName("Announcement Type")]
        public string AnnouncementType { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }
        [DisplayName("Submission Folder")]
        public string AttachmentFolder { get; set; }
        [DisplayName("Submission File")]
        public string AttachmentFile { get; set; }
    }
}
