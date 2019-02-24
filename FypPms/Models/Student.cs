using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FypPms.Models
{
    public partial class Student
    {
        [DisplayName("Student ID")]
        public int StudentId { get; set; }
        [DisplayName("Student ID")]
        public string AssignedId { get; set; }
        [DisplayName("Student Name")]
        public string StudentName { get; set; }
        [DisplayName("Student Status")]
        public string StudentStatus { get; set; }
        [DisplayName("Gender")]
        public string StudentGender { get; set; }
        [DisplayName("Student Specialization")]
        public string StudentSpecialization { get; set; }
        [DisplayName("Phone Number")]
        public string StudentPhone { get; set; }
        [DisplayName("Student Email")]
        public string StudentEmail { get; set; }
        [DisplayName("Alternate Email")]
        public string StudentAltEmail { get; set; }
        [DisplayName("Project")]
        public int? ProjectId { get; set; }
        [DisplayName("Batch")]
        public int? BatchId { get; set; }
        [DisplayName("User")]
        public int? UserId { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }

        public Project Project { get; set; }
        public Batch Batch { get; set; }
        public User User { get; set; }
    }
}
