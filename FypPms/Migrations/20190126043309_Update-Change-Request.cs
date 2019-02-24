using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class UpdateChangeRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectID",
                table: "ChangeRequest",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequest_ProjectID",
                table: "ChangeRequest",
                column: "ProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK__ChangeRequest__Project",
                table: "ChangeRequest",
                column: "ProjectID",
                principalTable: "Project",
                principalColumn: "ProjectID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__ChangeRequest__Project",
                table: "ChangeRequest");

            migrationBuilder.DropIndex(
                name: "IX_ChangeRequest_ProjectID",
                table: "ChangeRequest");

            migrationBuilder.DropColumn(
                name: "ProjectID",
                table: "ChangeRequest");
        }
    }
}
