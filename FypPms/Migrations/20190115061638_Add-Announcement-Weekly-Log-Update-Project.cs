using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class AddAnnouncementWeeklyLogUpdateProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Announcement",
                columns: table => new
                {
                    AnnouncementID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnnouncementSubject = table.Column<string>(unicode: false, nullable: true),
                    AnnouncementBody = table.Column<string>(unicode: false, nullable: true),
                    AnnouncementStatus = table.Column<string>(unicode: false, nullable: true),
                    AnnouncementType = table.Column<string>(unicode: false, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcement", x => x.AnnouncementID);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyLog",
                columns: table => new
                {
                    WeeklyLogID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StudentId = table.Column<string>(unicode: false, nullable: true),
                    SupervisorId = table.Column<string>(unicode: false, nullable: true),
                    CoSupervisorId = table.Column<string>(unicode: false, nullable: true),
                    WeeklyLogStatus = table.Column<string>(unicode: false, nullable: true),
                    WeeklyLogDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    WeeklyLogNumber = table.Column<int>(nullable: false),
                    WeeklyLogSession = table.Column<string>(unicode: false, nullable: true),
                    WorkDone = table.Column<string>(unicode: false, nullable: true),
                    WorkToBeDone = table.Column<string>(unicode: false, nullable: true),
                    Problem = table.Column<string>(unicode: false, nullable: true),
                    Comment = table.Column<string>(unicode: false, nullable: true),
                    ProjectID = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyLog", x => x.WeeklyLogID);
                    table.ForeignKey(
                        name: "FK__WeeklyLog__Project",
                        column: x => x.ProjectID,
                        principalTable: "Project",
                        principalColumn: "ProjectID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyLog_ProjectID",
                table: "WeeklyLog",
                column: "ProjectID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcement");

            migrationBuilder.DropTable(
                name: "WeeklyLog");
        }
    }
}
