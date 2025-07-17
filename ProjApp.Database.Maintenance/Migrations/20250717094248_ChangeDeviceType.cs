using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjApp.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDeviceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "manometr1verifications");

            migrationBuilder.AddColumn<string[]>(
                name: "manufacturers",
                table: "device_types",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "method_urls",
                table: "device_types",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "spec_urls",
                table: "device_types",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "manufacturers",
                table: "device_types");

            migrationBuilder.DropColumn(
                name: "method_urls",
                table: "device_types");

            migrationBuilder.DropColumn(
                name: "spec_urls",
                table: "device_types");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "manometr1verifications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
