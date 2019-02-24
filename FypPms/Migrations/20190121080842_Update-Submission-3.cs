using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class UpdateSubmission3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubmissionType",
                table: "Submission",
                newName: "SubmissionName");

            migrationBuilder.AddColumn<int>(
                name: "SubmissionTypeID",
                table: "Submission",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Submission_SubmissionTypeID",
                table: "Submission",
                column: "SubmissionTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK__Submission__SubmissionType",
                table: "Submission",
                column: "SubmissionTypeID",
                principalTable: "SubmissionType",
                principalColumn: "SubmissionTypeID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Submission__SubmissionType",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_SubmissionTypeID",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "SubmissionTypeID",
                table: "Submission");

            migrationBuilder.RenameColumn(
                name: "SubmissionName",
                table: "Submission",
                newName: "SubmissionType");
        }
    }
}
