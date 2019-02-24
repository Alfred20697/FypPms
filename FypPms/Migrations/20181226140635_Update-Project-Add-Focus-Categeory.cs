using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class UpdateProjectAddFocusCategeory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedId",
                table: "Project",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CompanyContact",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyContactPhone",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfStudent",
                table: "Project",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ProjectCollaboration",
                table: "Project",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProjectCompany",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProposedBy",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentOneSubtitle",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentOneWork",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentTwoSubtitle",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentTwoWork",
                table: "Project",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryName = table.Column<string>(unicode: false, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: true),
                    SpecializationID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryID);
                    table.ForeignKey(
                        name: "FK__Category__Specialization",
                        column: x => x.SpecializationID,
                        principalTable: "Specialization",
                        principalColumn: "SpecializationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Focus",
                columns: table => new
                {
                    FocusID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FocusName = table.Column<string>(unicode: false, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: true),
                    CategoryID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Focus", x => x.FocusID);
                    table.ForeignKey(
                        name: "FK__Focus__Category",
                        column: x => x.CategoryID,
                        principalTable: "Category",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_SpecializationID",
                table: "Category",
                column: "SpecializationID");

            migrationBuilder.CreateIndex(
                name: "IX_Focus_CategoryID",
                table: "Focus",
                column: "CategoryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Focus");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropColumn(
                name: "AssignedId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "CompanyContact",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "CompanyContactPhone",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "NumberOfStudent",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ProjectCollaboration",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ProjectCompany",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ProposedBy",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "StudentOneSubtitle",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "StudentOneWork",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "StudentTwoSubtitle",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "StudentTwoWork",
                table: "Project");
        }
    }
}
