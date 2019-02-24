using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FypPms.Models
{
    public partial class Proposal
    {
        [DisplayName("Proposal ID")]
        public int ProposalId { get; set; }
        [DisplayName("Proposal Sender")]
        public string Sender { get; set; }
        [DisplayName("Proposal Receiver")]
        public string Receiver { get; set; }
        [DisplayName("Proposal Comment")]
        public string ProposalComment { get; set; }
        [DisplayName("Proposal Status")]
        public string ProposalStatus { get; set; }
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
