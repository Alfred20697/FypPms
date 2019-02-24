using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FypPms.Models
{
    public class Moderation
    {
        [DisplayName("Moderation ID")]
        public int ModerationId { get; set; }
        [DisplayName("Project ID")]
        public int ProjectId { get; set; }
        [DisplayName("Supervisor ID")]
        public string SupervisorId { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }
    }
}
