using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjApp.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class DeviceTypeLinksVerificationMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "verification_method_id",
                table: "device_types",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_device_types_verification_method_id",
                table: "device_types",
                column: "verification_method_id");

            migrationBuilder.AddForeignKey(
                name: "fk_device_types_verification_methods_verification_method_id",
                table: "device_types",
                column: "verification_method_id",
                principalTable: "verification_methods",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_device_types_verification_methods_verification_method_id",
                table: "device_types");

            migrationBuilder.DropIndex(
                name: "ix_device_types_verification_method_id",
                table: "device_types");

            migrationBuilder.DropColumn(
                name: "verification_method_id",
                table: "device_types");
        }
    }
}
