using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FypPms.Models
{
    public partial class Project
    {
        public Project()
        {
            ProjectSpecialization = new HashSet<ProjectSpecialization>();
            Proposal = new HashSet<Proposal>();
            Review = new HashSet<Review>();
            Student = new HashSet<Student>();
            Supervision = new HashSet<Supervision>();
            WeeklyLog = new HashSet<WeeklyLog>();
            Submission = new HashSet<Submission>();
            ChangeRequest = new HashSet<ChangeRequest>();
        }
        
        public int ProjectId { get; set; }
        [DisplayName("Project ID")]
        public int AssignedId { get; set; }
        [DisplayName("Supervisor ID")]
        public string SupervisorId { get; set; }
        [DisplayName("Assigned Moderator")]
        public string ModeratorId { get; set; }
        [DisplayName("Co-Supervisor ID")]
        public string CoSupervisorId { get; set; }
        [DisplayName("Project Title")]
        public string ProjectTitle { get; set; }
        [DisplayName("Project Status")]
        public string ProjectStatus { get; set; }
        [DisplayName("Project Type")]
        public string ProjectType { get; set; }
        [DisplayName("Project Category")]
        public string ProjectCategory { get; set; }
        [DisplayName("Project Focus")]
        public string ProjectFocus { get; set; }
        [DisplayName("Project Collaboration")]
        public bool ProjectCollaboration { get; set; }
        [DisplayName("Project Company")]
        public string ProjectCompany { get; set; }
        [DisplayName("Company Contact Person")]
        public string CompanyContact { get; set; }
        [DisplayName("Company Contact Phone")]
        public string CompanyContactPhone { get; set; }
        [DisplayName("Proposed by")]
        public string ProposedBy { get; set; }
        [DisplayName("Number of Student")]
        public int NumberOfStudent { get; set; }
        [DisplayName("Student 1 Subtitle")]
        public string StudentOneSubtitle { get; set; }
        [DisplayName("Student 1 Work Distribution")]
        public string StudentOneWork { get; set; }
        [DisplayName("Student 2 Subtitle")]
        public string StudentTwoSubtitle { get; set; }
        [DisplayName("Student 2 Work Distribution")]
        public string StudentTwoWork { get; set; }
        [DisplayName("Moderator 1")]
        public string ModeratorOne{ get; set; }
        [DisplayName("Moderator 2")]
        public string ModeratorTwo { get; set; }
        [DisplayName("Moderator 3")]
        public string ModeratorThree { get; set; }
        [DisplayName("Project Stage")]
        public string ProjectStage { get; set; }
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }
        [DisplayName("Date Modified")]
        public DateTime? DateModified { get; set; }
        [DisplayName("Date Deleted")]
        public DateTime? DateDeleted { get; set; }

        public ICollection<ProjectSpecialization> ProjectSpecialization { get; set; }
        public ICollection<Proposal> Proposal { get; set; }
        public ICollection<Review> Review { get; set; }
        public ICollection<Student> Student { get; set; }
        public ICollection<Supervision> Supervision { get; set; }
        public ICollection<Requisition> Requisition { get; set; }
        public ICollection<WeeklyLog> WeeklyLog { get; set; }
        public ICollection<Submission> Submission { get; set; }
        public ICollection<ChangeRequest> ChangeRequest { get; set; }
    }
}
