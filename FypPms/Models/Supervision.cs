using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FypPms.Models
{
    public partial class Supervision
    {
        [DisplayName("Supervisor ID")]
        public int SupervisorId { get; set; }
        [DisplayName("Project ID")]
        public int ProjectId { get; set; }

        public Project Project { get; set; }
        public Supervisor Supervisor { get; set; }
    }
}
