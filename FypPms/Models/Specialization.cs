using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FypPms.Models
{
    public partial class Specialization
    {
        public Specialization()
        {
            ProjectSpecialization = new HashSet<ProjectSpecialization>();
            Category = new HashSet<Category>();
            Focus = new HashSet<Focus>();
        }

        [DisplayName("Specialization ID")]
        public int SpecializationId { get; set; }
        [DisplayName("Specialization Name")]
        public string SpecializationName { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }

        public ICollection<ProjectSpecialization> ProjectSpecialization { get; set; }
        public ICollection<Category> Category { get; set; }
        public ICollection<Focus> Focus { get; set; }
    }
}
