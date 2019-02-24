using Microsoft.EntityFrameworkCore.Migrations;

namespace FypPms.Migrations
{
    public partial class UpdateFocus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Focus__Category",
                table: "Focus");

            migrationBuilder.RenameColumn(
                name: "CategoryID",
                table: "Focus",
                newName: "SpecializationID");

            migrationBuilder.RenameIndex(
                name: "IX_Focus_CategoryID",
                table: "Focus",
                newName: "IX_Focus_SpecializationID");

            migrationBuilder.AddForeignKey(
                name: "FK__Focus__Specialization",
                table: "Focus",
                column: "SpecializationID",
                principalTable: "Specialization",
                principalColumn: "SpecializationID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Focus__Specialization",
                table: "Focus");

            migrationBuilder.RenameColumn(
                name: "SpecializationID",
                table: "Focus",
                newName: "CategoryID");

            migrationBuilder.RenameIndex(
                name: "IX_Focus_SpecializationID",
                table: "Focus",
                newName: "IX_Focus_CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK__Focus__Category",
                table: "Focus",
                column: "CategoryID",
                principalTable: "Category",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
