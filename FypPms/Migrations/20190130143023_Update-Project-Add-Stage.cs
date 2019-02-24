using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class UpdateProjectAddStage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectStage",
                table: "Project",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectStage",
                table: "Project");
        }
    }
}
