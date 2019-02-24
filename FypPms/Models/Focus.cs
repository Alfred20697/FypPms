using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FypPms.Models
{
    public class Focus
    {
        [DisplayName("Focus ID")]
        public int FocusId { get; set; }
        [DisplayName("Focus Name")]
        public string FocusName { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }
        [DisplayName("Specialization ID")]
        public int SpecializationId { get; set; }

        public Specialization Specialization { get; set; }
    }
}
