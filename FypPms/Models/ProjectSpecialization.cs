using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FypPms.Models
{
    public partial class ProjectSpecialization
    {
        [DisplayName("Project Specialization ID")]
        public int ProjectSpecializationId { get; set; }
        [DisplayName("Project ID")]
        public int ProjectId { get; set; }
        [DisplayName("Specialization ID")]
        public int SpecializationId { get; set; }
        [DisplayName("Project Description")]
        public string ProjectDescription { get; set; }
        [DisplayName("Project Objective")]
        public string ProjectObjective { get; set; }
        [DisplayName("Project Scope")]
        public string ProjectScope { get; set; }

        public Project Project { get; set; }
        public Specialization Specialization { get; set; }
    }
}
