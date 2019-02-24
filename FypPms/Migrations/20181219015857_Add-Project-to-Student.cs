using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class AddProjecttoStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectID",
                table: "Student",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Student_ProjectID",
                table: "Student",
                column: "ProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_ProjectID",
                table: "Student",
                column: "ProjectID",
                principalTable: "Project",
                principalColumn: "ProjectID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_ProjectID",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_ProjectID",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "ProjectID",
                table: "Student");
        }
    }
}
