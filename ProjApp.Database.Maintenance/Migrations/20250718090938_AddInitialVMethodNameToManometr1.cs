using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjApp.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialVMethodNameToManometr1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "initial_verification_name",
                table: "manometr1verifications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "initial_verification_name",
                table: "manometr1verifications");
        }
    }
}
