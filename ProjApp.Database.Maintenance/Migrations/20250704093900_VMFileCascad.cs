using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjApp.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class VMFileCascad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_verification_method_files_verification_methods_verification",
                table: "verification_method_files");

            migrationBuilder.AddForeignKey(
                name: "fk_verification_method_files_verification_methods_verification",
                table: "verification_method_files",
                column: "verification_method_id",
                principalTable: "verification_methods",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_verification_method_files_verification_methods_verification",
                table: "verification_method_files");

            migrationBuilder.AddForeignKey(
                name: "fk_verification_method_files_verification_methods_verification",
                table: "verification_method_files",
                column: "verification_method_id",
                principalTable: "verification_methods",
                principalColumn: "id");
        }
    }
}
