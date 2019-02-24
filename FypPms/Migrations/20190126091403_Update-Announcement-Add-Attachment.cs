using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class UpdateAnnouncementAddAttachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentFile",
                table: "Announcement",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentFolder",
                table: "Announcement",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentFile",
                table: "Announcement");

            migrationBuilder.DropColumn(
                name: "AttachmentFolder",
                table: "Announcement");
        }
    }
}
