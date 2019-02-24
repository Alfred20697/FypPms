using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FypPms.Models
{
    public class Session
    {
        [DisplayName("Session ID")]
        public int SessionId { get; set; }
        [DisplayName("Session Name")]
        public string SessionName { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }
    }
}
