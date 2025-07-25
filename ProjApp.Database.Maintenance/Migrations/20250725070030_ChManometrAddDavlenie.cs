using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjApp.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class ChManometrAddDavlenie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "device_modification",
                table: "manometr1verifications");

            migrationBuilder.DropColumn(
                name: "device_type_name",
                table: "manometr1verifications");

            migrationBuilder.DropColumn(
                name: "etalons_info",
                table: "manometr1verifications");

            migrationBuilder.DropColumn(
                name: "manufacture_year",
                table: "manometr1verifications");

            migrationBuilder.DropColumn(
                name: "owner_inn",
                table: "manometr1verifications");

            migrationBuilder.DropColumn(
                name: "verification_accuracy_checkup",
                table: "manometr1verifications");

            migrationBuilder.RenameColumn(
                name: "verifications_info",
                table: "manometr1verifications",
                newName: "visual_checkup");

            migrationBuilder.RenameColumn(
                name: "verification_visual_checkup",
                table: "manometr1verifications",
                newName: "test_checkup");

            migrationBuilder.RenameColumn(
                name: "verification_result_checkup",
                table: "manometr1verifications",
                newName: "accuracy_calculation");

            migrationBuilder.CreateTable(
                name: "davlenie1verifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    protocol_number = table.Column<string>(type: "text", nullable: false),
                    owner = table.Column<string>(type: "text", nullable: false),
                    temperature = table.Column<double>(type: "double precision", nullable: false),
                    humidity = table.Column<double>(type: "double precision", nullable: false),
                    pressure = table.Column<string>(type: "text", nullable: false),
                    visual_checkup = table.Column<string>(type: "text", nullable: false),
                    test_checkup = table.Column<string>(type: "text", nullable: false),
                    accuracy_calculation = table.Column<string>(type: "text", nullable: false),
                    verification_date = table.Column<DateOnly>(type: "date", nullable: false),
                    worker = table.Column<string>(type: "text", nullable: false),
                    device_type_number = table.Column<string>(type: "text", nullable: false),
                    device_serial = table.Column<string>(type: "text", nullable: false),
                    verification_group = table.Column<int>(type: "integer", nullable: false),
                    location = table.Column<int>(type: "integer", nullable: false),
                    verified_until_date = table.Column<DateOnly>(type: "date", nullable: false),
                    initial_verification_name = table.Column<string>(type: "text", nullable: false),
                    measurement_min = table.Column<double>(type: "double precision", nullable: false),
                    measurement_max = table.Column<double>(type: "double precision", nullable: false),
                    measurement_unit = table.Column<string>(type: "text", nullable: false),
                    pressure_inputs = table.Column<double[]>(type: "double precision[]", nullable: false),
                    etalon_values = table.Column<double[]>(type: "double precision[]", nullable: false),
                    device_values = table.Column<string>(type: "text", nullable: false),
                    actual_error = table.Column<string>(type: "text", nullable: false),
                    valid_error = table.Column<double>(type: "double precision", nullable: false),
                    variations = table.Column<double[]>(type: "double precision[]", nullable: false),
                    device_id = table.Column<Guid>(type: "uuid", nullable: true),
                    verification_method_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_davlenie1verifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_davlenie1verifications_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_davlenie1verifications_verification_methods_verification_me",
                        column: x => x.verification_method_id,
                        principalTable: "verification_methods",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "davlenie1verification_etalon",
                columns: table => new
                {
                    davlenie1verifications_id = table.Column<Guid>(type: "uuid", nullable: false),
                    etalons_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_davlenie1verification_etalon", x => new { x.davlenie1verifications_id, x.etalons_id });
                    table.ForeignKey(
                        name: "fk_davlenie1verification_etalon_davlenie1verifications_davleni",
                        column: x => x.davlenie1verifications_id,
                        principalTable: "davlenie1verifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_davlenie1verification_etalon_etalons_etalons_id",
                        column: x => x.etalons_id,
                        principalTable: "etalons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_davlenie1verification_etalon_etalons_id",
                table: "davlenie1verification_etalon",
                column: "etalons_id");

            migrationBuilder.CreateIndex(
                name: "ix_davlenie1verifications_device_id",
                table: "davlenie1verifications",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_davlenie1verifications_verification_method_id",
                table: "davlenie1verifications",
                column: "verification_method_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "davlenie1verification_etalon");

            migrationBuilder.DropTable(
                name: "davlenie1verifications");

            migrationBuilder.RenameColumn(
                name: "visual_checkup",
                table: "manometr1verifications",
                newName: "verifications_info");

            migrationBuilder.RenameColumn(
                name: "test_checkup",
                table: "manometr1verifications",
                newName: "verification_visual_checkup");

            migrationBuilder.RenameColumn(
                name: "accuracy_calculation",
                table: "manometr1verifications",
                newName: "verification_result_checkup");

            migrationBuilder.AddColumn<string>(
                name: "device_modification",
                table: "manometr1verifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "device_type_name",
                table: "manometr1verifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "etalons_info",
                table: "manometr1verifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "manufacture_year",
                table: "manometr1verifications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "owner_inn",
                table: "manometr1verifications",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "verification_accuracy_checkup",
                table: "manometr1verifications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
