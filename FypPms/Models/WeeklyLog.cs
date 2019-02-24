using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FypPms.Models
{
    public class WeeklyLog
    {
        [DisplayName("Weekly Log ID")]
        public int WeeklyLogId { get; set; }
        [DisplayName("Weekly Log Student")]
        public string StudentId { get; set; }
        [DisplayName("Weekly Log Supervisor")]
        public string SupervisorId { get; set; }
        [DisplayName("Weekly Log Co-Supervisor")]
        public string CoSupervisorId { get; set; }
        [DisplayName("Weekly Log Status")]
        public string WeeklyLogStatus { get; set; }
        [DisplayName("Weekly Log Date")]
        public DateTime WeeklyLogDate { get; set; }
        [DisplayName("Meeting No")]
        public int WeeklyLogNumber { get; set; }
        [DisplayName("Project Session")]
        public string WeeklyLogSession { get; set; }
        [DisplayName("Project Stages")]
        public string WeeklyLogStage { get; set; }
        [DisplayName("Work Done")]
        public string WorkDone { get; set; }
        [DisplayName("Work To Be Done")]
        public string WorkToBeDone { get; set; }
        [DisplayName("Problem")]
        public string Problem { get; set; }
        [DisplayName("Comment")]
        public string Comment { get; set; }
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
