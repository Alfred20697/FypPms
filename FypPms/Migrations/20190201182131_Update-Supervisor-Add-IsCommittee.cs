using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class UpdateSupervisorAddIsCommittee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCommittee",
                table: "Supervisor",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCommittee",
                table: "Supervisor");
        }
    }
}
