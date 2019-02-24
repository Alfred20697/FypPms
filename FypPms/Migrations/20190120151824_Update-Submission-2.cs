using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class UpdateSubmission2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Submission",
                newName: "SubmissionType");

            migrationBuilder.RenameColumn(
                name: "ReportSize",
                table: "Submission",
                newName: "SubmissionSize");

            migrationBuilder.RenameColumn(
                name: "Report",
                table: "Submission",
                newName: "SubmissionFolder");

            migrationBuilder.AddColumn<string>(
                name: "SubmissionFile",
                table: "Submission",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmissionFile",
                table: "Submission");

            migrationBuilder.RenameColumn(
                name: "SubmissionType",
                table: "Submission",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "SubmissionSize",
                table: "Submission",
                newName: "ReportSize");

            migrationBuilder.RenameColumn(
                name: "SubmissionFolder",
                table: "Submission",
                newName: "Report");
        }
    }
}
