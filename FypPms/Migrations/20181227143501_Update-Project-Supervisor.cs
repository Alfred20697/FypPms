using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class UpdateProjectSupervisor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoSupervisorId",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupervisorId",
                table: "Project",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoSupervisorId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "Project");
        }
    }
}
