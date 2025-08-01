using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjApp.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCheckupsStep1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "accuracy_calculation",
                table: "manometr1verifications");

            migrationBuilder.DropColumn(
                name: "test_checkup",
                table: "manometr1verifications");

            migrationBuilder.DropColumn(
                name: "visual_checkup",
                table: "manometr1verifications");

            migrationBuilder.DropColumn(
                name: "accuracy_calculation",
                table: "davlenie1verifications");

            migrationBuilder.DropColumn(
                name: "test_checkup",
                table: "davlenie1verifications");

            migrationBuilder.DropColumn(
                name: "visual_checkup",
                table: "davlenie1verifications");

            migrationBuilder.AddColumn<string>(
                name: "checkup_new",
                table: "verification_methods",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "checkup_new",
                table: "verification_methods");

            migrationBuilder.AddColumn<string>(
                name: "accuracy_calculation",
                table: "manometr1verifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "test_checkup",
                table: "manometr1verifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "visual_checkup",
                table: "manometr1verifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "accuracy_calculation",
                table: "davlenie1verifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "test_checkup",
                table: "davlenie1verifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "visual_checkup",
                table: "davlenie1verifications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
