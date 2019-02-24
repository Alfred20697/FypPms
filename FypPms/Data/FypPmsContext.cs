using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FypPms.Models
{
    public partial class FypPmsContext : DbContext
    {
        public FypPmsContext()
        {
        }

        public FypPmsContext(DbContextOptions<FypPmsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Announcement> Announcement { get; set; }
        public virtual DbSet<Batch> Batch { get; set; }
        public virtual DbSet<ChangeRequest> ChangeRequest { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Confirmation> Confirmation { get; set; }
        public virtual DbSet<Coordinator> Coordinator { get; set; }
        public virtual DbSet<Focus> Focus { get; set; }
        public virtual DbSet<Moderation> Moderation { get; set; }
        public virtual DbSet<Requisition> Requisition { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectSpecialization> ProjectSpecialization { get; set; }
        public virtual DbSet<Proposal> Proposal { get; set; }
        public virtual DbSet<Review> Review { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RolePermission> RolePermission { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<Specialization> Specialization { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Submission> Submission { get; set; }
        public virtual DbSet<SubmissionType> SubmissionType { get; set; }
        public virtual DbSet<Supervision> Supervision { get; set; }
        public virtual DbSet<Supervisor> Supervisor { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<WeeklyLog> WeeklyLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=fyppms;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.Property(e => e.AnnouncementId).HasColumnName("AnnouncementID");

                entity.Property(e => e.AnnouncementType).IsUnicode(false);

                entity.Property(e => e.AnnouncementStatus).IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");
            });

            modelBuilder.Entity<Batch>(entity =>
            {
                entity.Property(e => e.BatchId).HasColumnName("BatchID");

                entity.Property(e => e.BatchEndDate).HasColumnType("datetime");

                entity.Property(e => e.BatchName).IsUnicode(false);

                entity.Property(e => e.BatchStartDate).HasColumnType("datetime");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName).IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.SpecializationId).HasColumnName("SpecializationID");

                entity.HasOne(d => d.Specialization)
                    .WithMany(p => p.Category)
                    .HasForeignKey(d => d.SpecializationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Category__Specialization");
            });

            modelBuilder.Entity<ChangeRequest>(entity =>
            {
                entity.Property(e => e.ChangeRequestId).HasColumnName("ChangeRequestID");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Objective).IsUnicode(false);

                entity.Property(e => e.Scope).IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ChangeRequest)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChangeRequest__Project");
            });

            modelBuilder.Entity<Confirmation>(entity =>
            {
                entity.Property(e => e.ConfirmationId).HasColumnName("ConfirmationID");

                entity.Property(e => e.ConfirmationStatus).IsUnicode(false);

                entity.Property(e => e.StudentId).IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");
            });

            modelBuilder.Entity<Coordinator>(entity =>
            {
                entity.Property(e => e.CoordinatorId).HasColumnName("CoordinatorID");

                entity.Property(e => e.AssignedId)
                    .IsRequired()
                    .HasColumnName("AssignedID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.CoordinatorEmail).IsUnicode(false);

                entity.Property(e => e.CoordinatorGender)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CoordinatorName).IsUnicode(false);

                entity.Property(e => e.CoordinatorPhone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CoordinatorStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Coordinator)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Coordinator__User");
            });

            modelBuilder.Entity<Focus>(entity =>
            {
                entity.Property(e => e.FocusId).HasColumnName("FocusID");

                entity.Property(e => e.FocusName).IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.SpecializationId).HasColumnName("SpecializationID");

                entity.HasOne(d => d.Specialization)
                    .WithMany(p => p.Focus)
                    .HasForeignKey(d => d.SpecializationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Focus__Specialization");
            });

            modelBuilder.Entity<Moderation>(entity =>
            {
                entity.Property(e => e.ModerationId).HasColumnName("ModerationID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");
            });

            modelBuilder.Entity<Requisition>(entity =>
            {
                entity.Property(e => e.RequisitionId).HasColumnName("RequisitionID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.RequisitionStatus).IsUnicode(false);

                entity.Property(e => e.Receiver).IsUnicode(false);

                entity.Property(e => e.Sender).IsUnicode(false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Requisition)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Requisition__Project");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.PermissionId).HasColumnName("PermissionID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.PermissionDescription).IsUnicode(false);

                entity.Property(e => e.PermissionName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.ProjectCategory).IsUnicode(false);

                entity.Property(e => e.ProjectFocus).IsUnicode(false);

                entity.Property(e => e.ProjectStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectTitle).IsUnicode(false);

                entity.Property(e => e.ProjectType)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ProjectSpecialization>(entity =>
            {
                entity.Property(e => e.ProjectSpecializationId).HasColumnName("ProjectSpecializationID");

                entity.Property(e => e.ProjectDescription).IsUnicode(false);

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.ProjectObjective).IsUnicode(false);

                entity.Property(e => e.ProjectScope).IsUnicode(false);

                entity.Property(e => e.SpecializationId).HasColumnName("SpecializationID");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectSpecialization)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectSp__Proje__68487DD7");

                entity.HasOne(d => d.Specialization)
                    .WithMany(p => p.ProjectSpecialization)
                    .HasForeignKey(d => d.SpecializationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProjectSp__Speci__693CA210");
            });

            modelBuilder.Entity<Proposal>(entity =>
            {
                entity.Property(e => e.ProposalId).HasColumnName("ProposalID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.ProposalStatus).IsUnicode(false);

                entity.Property(e => e.Receiver).IsUnicode(false);

                entity.Property(e => e.Sender).IsUnicode(false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Proposal)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Proposal__Projec__6C190EBB");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.Property(e => e.ReviewId).HasColumnName("ReviewID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.ReviewStatus).IsUnicode(false);

                entity.Property(e => e.Reviewer).IsUnicode(false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Review)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Review__ProjectI__6EF57B66");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.RoleDescription).IsUnicode(false);

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.PermissionId });

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.PermissionId).HasColumnName("PermissionID");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.RolePermission)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RolePermi__Permi__76969D2E");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RolePermission)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RolePermi__RoleI__75A278F5");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.Property(e => e.SessionId).HasColumnName("SessionID");

                entity.Property(e => e.SessionName).IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");
            });

            modelBuilder.Entity<Specialization>(entity =>
            {
                entity.Property(e => e.SpecializationId).HasColumnName("SpecializationID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.SpecializationName).IsUnicode(false);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(e => e.StudentId).HasColumnName("StudentID");

                entity.Property(e => e.AssignedId)
                    .IsRequired()
                    .HasColumnName("AssignedID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BatchId).HasColumnName("BatchID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.StudentAltEmail).IsUnicode(false);

                entity.Property(e => e.StudentEmail).IsUnicode(false);

                entity.Property(e => e.StudentGender)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StudentName).IsUnicode(false);

                entity.Property(e => e.StudentPhone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StudentSpecialization).IsUnicode(false);

                entity.Property(e => e.StudentStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.BatchId)
                    .HasConstraintName("FK__Student__BatchID__5FB337D6");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Student_ProjectID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Student__UserID__60A75C0F");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Submission)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Submission__Project");
            });

            modelBuilder.Entity<SubmissionType>(entity =>
            {
                entity.Property(e => e.SubmissionTypeId).HasColumnName("SubmissionTypeID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.GraceDate).HasColumnType("datetime");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.SubmissionType)
                    .HasForeignKey(d => d.BatchId)
                    .HasConstraintName("FK__SubmissionType__Batch");
            });

            modelBuilder.Entity<Supervision>(entity =>
            {
                entity.HasKey(e => new { e.SupervisorId, e.ProjectId });

                entity.Property(e => e.SupervisorId).HasColumnName("SupervisorID");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Supervision)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Supervisi__Proje__7A672E12");

                entity.HasOne(d => d.Supervisor)
                    .WithMany(p => p.Supervision)
                    .HasForeignKey(d => d.SupervisorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Supervisi__Super__797309D9");
            });

            modelBuilder.Entity<Supervisor>(entity =>
            {
                entity.Property(e => e.SupervisorId).HasColumnName("SupervisorID");

                entity.Property(e => e.AssignedId)
                    .IsRequired()
                    .HasColumnName("AssignedID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.SupervisorEmail).IsUnicode(false);

                entity.Property(e => e.SupervisorGender)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SupervisorName).IsUnicode(false);

                entity.Property(e => e.SupervisorPhone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SupervisorSpecialization).IsUnicode(false);

                entity.Property(e => e.SupervisorStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Supervisor)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Superviso__UserI__5CD6CB2B");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserStatus).HasMaxLength(50);

                entity.Property(e => e.UserType).HasMaxLength(50);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserRole__RoleID__72C60C4A");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserRole__UserID__71D1E811");
            });

            modelBuilder.Entity<WeeklyLog>(entity =>
            {
                entity.Property(e => e.WeeklyLogId).HasColumnName("WeeklyLogID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateDeleted).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.WeeklyLogDate).HasColumnType("datetime");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.WeeklyLogStatus).IsUnicode(false);

                entity.Property(e => e.StudentId).IsUnicode(false);

                entity.Property(e => e.SupervisorId).IsUnicode(false);

                entity.Property(e => e.CoSupervisorId).IsUnicode(false);

                entity.Property(e => e.WeeklyLogSession).IsUnicode(false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.WeeklyLog)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__WeeklyLog__Project");
            });
        }
    }
}
