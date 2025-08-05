using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjApp.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCheckups2Final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "checkups",
                table: "verification_methods");
                
            migrationBuilder.RenameColumn(
                name: "checkups_new",
                table: "verification_methods",
                newName: "checkups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // rename column checkups to checkup_new
            migrationBuilder.RenameColumn(
                name: "checkups",
                table: "verification_methods",
                newName: "checkup_new");
        }
    }
}
