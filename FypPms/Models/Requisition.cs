using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FypPms.Models
{
    public class Requisition
    {
        [DisplayName("Requisition ID")]
        public int RequisitionId { get; set; }
        [DisplayName("Requisition Sender")]
        public string Sender { get; set; }
        [DisplayName("Requisition Receiver")]
        public string Receiver { get; set; }
        [DisplayName("Requisition Status")]
        public string RequisitionStatus { get; set; }
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
