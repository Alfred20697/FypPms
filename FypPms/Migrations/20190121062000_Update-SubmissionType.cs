using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class UpdateSubmissionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BatchId",
                table: "SubmissionType",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionType_BatchId",
                table: "SubmissionType",
                column: "BatchId");

            migrationBuilder.AddForeignKey(
                name: "FK__SubmissionType__Batch",
                table: "SubmissionType",
                column: "BatchId",
                principalTable: "Batch",
                principalColumn: "BatchID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__SubmissionType__Batch",
                table: "SubmissionType");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionType_BatchId",
                table: "SubmissionType");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "SubmissionType");
        }
    }
}
