using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class UpdateProjectModerator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecommendedModerator",
                table: "Project",
                newName: "ModeratorTwo");

            migrationBuilder.AddColumn<string>(
                name: "ModeratorOne",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModeratorThree",
                table: "Project",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModeratorOne",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ModeratorThree",
                table: "Project");

            migrationBuilder.RenameColumn(
                name: "ModeratorTwo",
                table: "Project",
                newName: "RecommendedModerator");
        }
    }
}
