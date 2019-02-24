using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class UpdateModerationSubmission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Moderation__Project",
                table: "Moderation");

            migrationBuilder.DropForeignKey(
                name: "FK__Moderation__Supervisor",
                table: "Moderation");

            migrationBuilder.DropForeignKey(
                name: "FK__Submission__SubmissionType",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_SubmissionTypeID",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Moderation_ProjectID",
                table: "Moderation");

            migrationBuilder.DropIndex(
                name: "IX_Moderation_SupervisorID",
                table: "Moderation");

            migrationBuilder.RenameColumn(
                name: "SubmissionTypeID",
                table: "Submission",
                newName: "SubmissionTypeId");

            migrationBuilder.RenameColumn(
                name: "SupervisorID",
                table: "Moderation",
                newName: "SupervisorId");

            migrationBuilder.RenameColumn(
                name: "ProjectID",
                table: "Moderation",
                newName: "ProjectId");

            migrationBuilder.AlterColumn<string>(
                name: "SupervisorId",
                table: "Moderation",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubmissionTypeId",
                table: "Submission",
                newName: "SubmissionTypeID");

            migrationBuilder.RenameColumn(
                name: "SupervisorId",
                table: "Moderation",
                newName: "SupervisorID");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Moderation",
                newName: "ProjectID");

            migrationBuilder.AlterColumn<int>(
                name: "SupervisorID",
                table: "Moderation",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submission_SubmissionTypeID",
                table: "Submission",
                column: "SubmissionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Moderation_ProjectID",
                table: "Moderation",
                column: "ProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Moderation_SupervisorID",
                table: "Moderation",
                column: "SupervisorID");

            migrationBuilder.AddForeignKey(
                name: "FK__Moderation__Project",
                table: "Moderation",
                column: "ProjectID",
                principalTable: "Project",
                principalColumn: "ProjectID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__Moderation__Supervisor",
                table: "Moderation",
                column: "SupervisorID",
                principalTable: "Supervisor",
                principalColumn: "SupervisorID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__Submission__SubmissionType",
                table: "Submission",
                column: "SubmissionTypeID",
                principalTable: "SubmissionType",
                principalColumn: "SubmissionTypeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
