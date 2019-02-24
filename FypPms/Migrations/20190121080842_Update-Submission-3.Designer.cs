﻿// <auto-generated />
using System;
using FypPms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FypPms.Migrations
{
    [DbContext(typeof(FypPmsContext))]
    [Migration("20190121080842_Update-Submission-3")]
    partial class UpdateSubmission3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FypPms.Models.Announcement", b =>
                {
                    b.Property<int>("AnnouncementId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AnnouncementID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AnnouncementBody");

                    b.Property<string>("AnnouncementStatus")
                        .IsUnicode(false);

                    b.Property<string>("AnnouncementSubject");

                    b.Property<string>("AnnouncementType")
                        .IsUnicode(false);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.HasKey("AnnouncementId");

                    b.ToTable("Announcement");
                });

            modelBuilder.Entity("FypPms.Models.Batch", b =>
                {
                    b.Property<int>("BatchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("BatchID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("BatchEndDate")
                        .HasColumnType("datetime");

                    b.Property<string>("BatchName")
                        .IsUnicode(false);

                    b.Property<DateTime>("BatchStartDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.HasKey("BatchId");

                    b.ToTable("Batch");
                });

            modelBuilder.Entity("FypPms.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CategoryID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CategoryName")
                        .IsUnicode(false);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<int>("SpecializationId")
                        .HasColumnName("SpecializationID");

                    b.HasKey("CategoryId");

                    b.HasIndex("SpecializationId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("FypPms.Models.Confirmation", b =>
                {
                    b.Property<int>("ConfirmationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ConfirmationID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConfirmationStatus")
                        .IsUnicode(false);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<int>("ProjectId");

                    b.Property<string>("StudentId")
                        .IsUnicode(false);

                    b.HasKey("ConfirmationId");

                    b.ToTable("Confirmation");
                });

            modelBuilder.Entity("FypPms.Models.Coordinator", b =>
                {
                    b.Property<int>("CoordinatorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CoordinatorID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AssignedId")
                        .IsRequired()
                        .HasColumnName("AssignedID")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("CoordinatorEmail")
                        .IsUnicode(false);

                    b.Property<string>("CoordinatorGender")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("CoordinatorName")
                        .IsUnicode(false);

                    b.Property<string>("CoordinatorPhone")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("CoordinatorStatus")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<int?>("UserId")
                        .HasColumnName("UserID");

                    b.HasKey("CoordinatorId");

                    b.HasIndex("UserId");

                    b.ToTable("Coordinator");
                });

            modelBuilder.Entity("FypPms.Models.Focus", b =>
                {
                    b.Property<int>("FocusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("FocusID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<string>("FocusName")
                        .IsUnicode(false);

                    b.Property<int>("SpecializationId")
                        .HasColumnName("SpecializationID");

                    b.HasKey("FocusId");

                    b.HasIndex("SpecializationId");

                    b.ToTable("Focus");
                });

            modelBuilder.Entity("FypPms.Models.Moderation", b =>
                {
                    b.Property<int>("ModerationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ModerationID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<int>("ProjectId")
                        .HasColumnName("ProjectID");

                    b.Property<int>("SupervisorId")
                        .HasColumnName("SupervisorID");

                    b.HasKey("ModerationId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("SupervisorId");

                    b.ToTable("Moderation");
                });

            modelBuilder.Entity("FypPms.Models.Permission", b =>
                {
                    b.Property<int>("PermissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("PermissionID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<string>("PermissionDescription")
                        .IsUnicode(false);

                    b.Property<string>("PermissionName")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.HasKey("PermissionId");

                    b.ToTable("Permission");
                });

            modelBuilder.Entity("FypPms.Models.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ProjectID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AssignedId");

                    b.Property<string>("CoSupervisorId");

                    b.Property<string>("CompanyContact");

                    b.Property<string>("CompanyContactPhone");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<string>("ModeratorOne");

                    b.Property<string>("ModeratorThree");

                    b.Property<string>("ModeratorTwo");

                    b.Property<int>("NumberOfStudent");

                    b.Property<string>("ProjectCategory")
                        .IsUnicode(false);

                    b.Property<bool>("ProjectCollaboration");

                    b.Property<string>("ProjectCompany");

                    b.Property<string>("ProjectFocus")
                        .IsUnicode(false);

                    b.Property<string>("ProjectStatus")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("ProjectTitle")
                        .IsUnicode(false);

                    b.Property<string>("ProjectType")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("ProposedBy");

                    b.Property<string>("StudentOneSubtitle");

                    b.Property<string>("StudentOneWork");

                    b.Property<string>("StudentTwoSubtitle");

                    b.Property<string>("StudentTwoWork");

                    b.Property<string>("SupervisorId");

                    b.HasKey("ProjectId");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("FypPms.Models.ProjectSpecialization", b =>
                {
                    b.Property<int>("ProjectSpecializationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ProjectSpecializationID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ProjectDescription")
                        .IsUnicode(false);

                    b.Property<int>("ProjectId")
                        .HasColumnName("ProjectID");

                    b.Property<string>("ProjectObjective")
                        .IsUnicode(false);

                    b.Property<string>("ProjectScope")
                        .IsUnicode(false);

                    b.Property<int>("SpecializationId")
                        .HasColumnName("SpecializationID");

                    b.HasKey("ProjectSpecializationId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("SpecializationId");

                    b.ToTable("ProjectSpecialization");
                });

            modelBuilder.Entity("FypPms.Models.Proposal", b =>
                {
                    b.Property<int>("ProposalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ProposalID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<int>("ProjectId")
                        .HasColumnName("ProjectID");

                    b.Property<string>("ProposalComment");

                    b.Property<string>("ProposalStatus")
                        .IsUnicode(false);

                    b.Property<string>("Receiver")
                        .IsUnicode(false);

                    b.Property<string>("Sender")
                        .IsUnicode(false);

                    b.HasKey("ProposalId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Proposal");
                });

            modelBuilder.Entity("FypPms.Models.Requisition", b =>
                {
                    b.Property<int>("RequisitionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RequisitionID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<int>("ProjectId")
                        .HasColumnName("ProjectID");

                    b.Property<string>("Receiver")
                        .IsUnicode(false);

                    b.Property<string>("RequisitionStatus")
                        .IsUnicode(false);

                    b.Property<string>("Sender")
                        .IsUnicode(false);

                    b.HasKey("RequisitionId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Requisition");
                });

            modelBuilder.Entity("FypPms.Models.Review", b =>
                {
                    b.Property<int>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ReviewID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<int>("ProjectId")
                        .HasColumnName("ProjectID");

                    b.Property<string>("ReviewComment");

                    b.Property<string>("ReviewStatus")
                        .IsUnicode(false);

                    b.Property<string>("Reviewer")
                        .IsUnicode(false);

                    b.HasKey("ReviewId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Review");
                });

            modelBuilder.Entity("FypPms.Models.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RoleID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<string>("RoleDescription")
                        .IsUnicode(false);

                    b.Property<string>("RoleName")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.HasKey("RoleId");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("FypPms.Models.RolePermission", b =>
                {
                    b.Property<int>("RoleId")
                        .HasColumnName("RoleID");

                    b.Property<int>("PermissionId")
                        .HasColumnName("PermissionID");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("RolePermission");
                });

            modelBuilder.Entity("FypPms.Models.Session", b =>
                {
                    b.Property<int>("SessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SessionID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<string>("SessionName")
                        .IsUnicode(false);

                    b.HasKey("SessionId");

                    b.ToTable("Session");
                });

            modelBuilder.Entity("FypPms.Models.Specialization", b =>
                {
                    b.Property<int>("SpecializationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SpecializationID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<string>("SpecializationName")
                        .IsUnicode(false);

                    b.HasKey("SpecializationId");

                    b.ToTable("Specialization");
                });

            modelBuilder.Entity("FypPms.Models.Student", b =>
                {
                    b.Property<int>("StudentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("StudentID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AssignedId")
                        .IsRequired()
                        .HasColumnName("AssignedID")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<int?>("BatchId")
                        .HasColumnName("BatchID");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<int?>("ProjectId")
                        .HasColumnName("ProjectID");

                    b.Property<string>("StudentAltEmail")
                        .IsUnicode(false);

                    b.Property<string>("StudentEmail")
                        .IsUnicode(false);

                    b.Property<string>("StudentGender")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("StudentName")
                        .IsUnicode(false);

                    b.Property<string>("StudentPhone")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("StudentSpecialization")
                        .IsUnicode(false);

                    b.Property<string>("StudentStatus")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<int?>("UserId")
                        .HasColumnName("UserID");

                    b.HasKey("StudentId");

                    b.HasIndex("BatchId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("Student");
                });

            modelBuilder.Entity("FypPms.Models.Submission", b =>
                {
                    b.Property<int>("SubmissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SubmissionID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<int>("ProjectId")
                        .HasColumnName("ProjectID");

                    b.Property<string>("SubmissionFile");

                    b.Property<string>("SubmissionFolder");

                    b.Property<string>("SubmissionName");

                    b.Property<long>("SubmissionSize");

                    b.Property<string>("SubmissionStatus");

                    b.Property<int>("SubmissionTypeId")
                        .HasColumnName("SubmissionTypeID");

                    b.Property<DateTime>("UploadDate");

                    b.HasKey("SubmissionId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("SubmissionTypeId");

                    b.ToTable("Submission");
                });

            modelBuilder.Entity("FypPms.Models.SubmissionType", b =>
                {
                    b.Property<int>("SubmissionTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SubmissionTypeID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("BatchId");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("GraceDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Name");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Status");

                    b.HasKey("SubmissionTypeId");

                    b.HasIndex("BatchId");

                    b.ToTable("SubmissionType");
                });

            modelBuilder.Entity("FypPms.Models.Supervision", b =>
                {
                    b.Property<int>("SupervisorId")
                        .HasColumnName("SupervisorID");

                    b.Property<int>("ProjectId")
                        .HasColumnName("ProjectID");

                    b.HasKey("SupervisorId", "ProjectId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Supervision");
                });

            modelBuilder.Entity("FypPms.Models.Supervisor", b =>
                {
                    b.Property<int>("SupervisorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SupervisorID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AssignedId")
                        .IsRequired()
                        .HasColumnName("AssignedID")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<string>("SupervisorEmail")
                        .IsUnicode(false);

                    b.Property<string>("SupervisorGender")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("SupervisorName")
                        .IsUnicode(false);

                    b.Property<string>("SupervisorPhone")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("SupervisorSpecialization")
                        .IsUnicode(false);

                    b.Property<string>("SupervisorStatus")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<int?>("UserId")
                        .HasColumnName("UserID");

                    b.HasKey("SupervisorId");

                    b.HasIndex("UserId");

                    b.ToTable("Supervisor");
                });

            modelBuilder.Entity("FypPms.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("UserID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("UserPassword")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("UserStatus")
                        .HasMaxLength(50);

                    b.Property<string>("UserType")
                        .HasMaxLength(50);

                    b.HasKey("UserId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("FypPms.Models.UserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnName("UserID");

                    b.Property<int>("RoleId")
                        .HasColumnName("RoleID");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("FypPms.Models.WeeklyLog", b =>
                {
                    b.Property<int>("WeeklyLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("WeeklyLogID")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CoSupervisorId")
                        .IsUnicode(false);

                    b.Property<string>("Comment");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime");

                    b.Property<string>("Problem");

                    b.Property<int>("ProjectId")
                        .HasColumnName("ProjectID");

                    b.Property<string>("StudentId")
                        .IsUnicode(false);

                    b.Property<string>("SupervisorId")
                        .IsUnicode(false);

                    b.Property<DateTime>("WeeklyLogDate")
                        .HasColumnType("datetime");

                    b.Property<int>("WeeklyLogNumber");

                    b.Property<string>("WeeklyLogSession")
                        .IsUnicode(false);

                    b.Property<string>("WeeklyLogStatus")
                        .IsUnicode(false);

                    b.Property<string>("WorkDone");

                    b.Property<string>("WorkToBeDone");

                    b.HasKey("WeeklyLogId");

                    b.HasIndex("ProjectId");

                    b.ToTable("WeeklyLog");
                });

            modelBuilder.Entity("FypPms.Models.Category", b =>
                {
                    b.HasOne("FypPms.Models.Specialization", "Specialization")
                        .WithMany("Category")
                        .HasForeignKey("SpecializationId")
                        .HasConstraintName("FK__Category__Specialization");
                });

            modelBuilder.Entity("FypPms.Models.Coordinator", b =>
                {
                    b.HasOne("FypPms.Models.User", "User")
                        .WithMany("Coordinator")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__Coordinator__User");
                });

            modelBuilder.Entity("FypPms.Models.Focus", b =>
                {
                    b.HasOne("FypPms.Models.Specialization", "Specialization")
                        .WithMany("Focus")
                        .HasForeignKey("SpecializationId")
                        .HasConstraintName("FK__Focus__Specialization");
                });

            modelBuilder.Entity("FypPms.Models.Moderation", b =>
                {
                    b.HasOne("FypPms.Models.Project", "Project")
                        .WithMany("Moderation")
                        .HasForeignKey("ProjectId")
                        .HasConstraintName("FK__Moderation__Project");

                    b.HasOne("FypPms.Models.Supervisor", "Supervisor")
                        .WithMany("Moderation")
                        .HasForeignKey("SupervisorId")
                        .HasConstraintName("FK__Moderation__Supervisor");
                });

            modelBuilder.Entity("FypPms.Models.ProjectSpecialization", b =>
                {
                    b.HasOne("FypPms.Models.Project", "Project")
                        .WithMany("ProjectSpecialization")
                        .HasForeignKey("ProjectId")
                        .HasConstraintName("FK__ProjectSp__Proje__68487DD7");

                    b.HasOne("FypPms.Models.Specialization", "Specialization")
                        .WithMany("ProjectSpecialization")
                        .HasForeignKey("SpecializationId")
                        .HasConstraintName("FK__ProjectSp__Speci__693CA210");
                });

            modelBuilder.Entity("FypPms.Models.Proposal", b =>
                {
                    b.HasOne("FypPms.Models.Project", "Project")
                        .WithMany("Proposal")
                        .HasForeignKey("ProjectId")
                        .HasConstraintName("FK__Proposal__Projec__6C190EBB");
                });

            modelBuilder.Entity("FypPms.Models.Requisition", b =>
                {
                    b.HasOne("FypPms.Models.Project", "Project")
                        .WithMany("Requisition")
                        .HasForeignKey("ProjectId")
                        .HasConstraintName("FK__Requisition__Project");
                });

            modelBuilder.Entity("FypPms.Models.Review", b =>
                {
                    b.HasOne("FypPms.Models.Project", "Project")
                        .WithMany("Review")
                        .HasForeignKey("ProjectId")
                        .HasConstraintName("FK__Review__ProjectI__6EF57B66");
                });

            modelBuilder.Entity("FypPms.Models.RolePermission", b =>
                {
                    b.HasOne("FypPms.Models.Permission", "Permission")
                        .WithMany("RolePermission")
                        .HasForeignKey("PermissionId")
                        .HasConstraintName("FK__RolePermi__Permi__76969D2E");

                    b.HasOne("FypPms.Models.Role", "Role")
                        .WithMany("RolePermission")
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__RolePermi__RoleI__75A278F5");
                });

            modelBuilder.Entity("FypPms.Models.Student", b =>
                {
                    b.HasOne("FypPms.Models.Batch", "Batch")
                        .WithMany("Student")
                        .HasForeignKey("BatchId")
                        .HasConstraintName("FK__Student__BatchID__5FB337D6");

                    b.HasOne("FypPms.Models.Project", "Project")
                        .WithMany("Student")
                        .HasForeignKey("ProjectId")
                        .HasConstraintName("FK_Student_ProjectID");

                    b.HasOne("FypPms.Models.User", "User")
                        .WithMany("Student")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__Student__UserID__60A75C0F");
                });

            modelBuilder.Entity("FypPms.Models.Submission", b =>
                {
                    b.HasOne("FypPms.Models.Project", "Project")
                        .WithMany("Submission")
                        .HasForeignKey("ProjectId")
                        .HasConstraintName("FK__Submission__Project");

                    b.HasOne("FypPms.Models.SubmissionType", "SubmissionType")
                        .WithMany("Submission")
                        .HasForeignKey("SubmissionTypeId")
                        .HasConstraintName("FK__Submission__SubmissionType");
                });

            modelBuilder.Entity("FypPms.Models.SubmissionType", b =>
                {
                    b.HasOne("FypPms.Models.Batch", "Batch")
                        .WithMany("SubmissionType")
                        .HasForeignKey("BatchId")
                        .HasConstraintName("FK__SubmissionType__Batch");
                });

            modelBuilder.Entity("FypPms.Models.Supervision", b =>
                {
                    b.HasOne("FypPms.Models.Project", "Project")
                        .WithMany("Supervision")
                        .HasForeignKey("ProjectId")
                        .HasConstraintName("FK__Supervisi__Proje__7A672E12");

                    b.HasOne("FypPms.Models.Supervisor", "Supervisor")
                        .WithMany("Supervision")
                        .HasForeignKey("SupervisorId")
                        .HasConstraintName("FK__Supervisi__Super__797309D9");
                });

            modelBuilder.Entity("FypPms.Models.Supervisor", b =>
                {
                    b.HasOne("FypPms.Models.User", "User")
                        .WithMany("Supervisor")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__Superviso__UserI__5CD6CB2B");
                });

            modelBuilder.Entity("FypPms.Models.UserRole", b =>
                {
                    b.HasOne("FypPms.Models.Role", "Role")
                        .WithMany("UserRole")
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__UserRole__RoleID__72C60C4A");

                    b.HasOne("FypPms.Models.User", "User")
                        .WithMany("UserRole")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserRole__UserID__71D1E811");
                });

            modelBuilder.Entity("FypPms.Models.WeeklyLog", b =>
                {
                    b.HasOne("FypPms.Models.Project", "Project")
                        .WithMany("WeeklyLog")
                        .HasForeignKey("ProjectId")
                        .HasConstraintName("FK__WeeklyLog__Project");
                });
#pragma warning restore 612, 618
        }
    }
}
