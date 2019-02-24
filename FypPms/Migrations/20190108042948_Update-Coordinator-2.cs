using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class UpdateCoordinator2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coordinator",
                columns: table => new
                {
                    CoordinatorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssignedID = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    CoordinatorName = table.Column<string>(unicode: false, nullable: true),
                    CoordinatorStatus = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    CoordinatorGender = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    CoordinatorPhone = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    CoordinatorEmail = table.Column<string>(unicode: false, nullable: true),
                    UserID = table.Column<int>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordinator", x => x.CoordinatorID);
                    table.ForeignKey(
                        name: "FK__Coordinator__User",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coordinator_UserID",
                table: "Coordinator",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coordinator");
        }
    }
}
