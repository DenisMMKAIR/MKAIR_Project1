using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.FGIS.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "device_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_type_number = table.Column<string>(type: "text", nullable: false),
                    device_type_notation = table.Column<string>(type: "text", nullable: false),
                    device_type_title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_serial_number = table.Column<string>(type: "text", nullable: false),
                    device_manufactured_year = table.Column<long>(type: "bigint", nullable: false),
                    device_modification = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_devices", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "verifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_name = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    next_date = table.Column<DateOnly>(type: "date", nullable: false),
                    verification_name = table.Column<string>(type: "text", nullable: false),
                    applicable = table.Column<bool>(type: "boolean", nullable: false),
                    additional_info = table.Column<string>(type: "text", nullable: false),
                    device_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    device_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_verifications_device_types_device_type_id",
                        column: x => x.device_type_id,
                        principalTable: "device_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_verifications_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "etalons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    reg_number = table.Column<string>(type: "text", nullable: false),
                    type_number = table.Column<string>(type: "text", nullable: false),
                    type_title = table.Column<string>(type: "text", nullable: false),
                    notation = table.Column<string>(type: "text", nullable: false),
                    modification = table.Column<string>(type: "text", nullable: false),
                    manufacture_num = table.Column<string>(type: "text", nullable: false),
                    manufacture_year = table.Column<long>(type: "bigint", nullable: false),
                    rank_code = table.Column<string>(type: "text", nullable: false),
                    rank_title = table.Column<string>(type: "text", nullable: false),
                    schema_title = table.Column<string>(type: "text", nullable: false),
                    verification_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_etalons", x => x.id);
                    table.ForeignKey(
                        name: "fk_etalons_verifications_verification_id",
                        column: x => x.verification_id,
                        principalTable: "verifications",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_etalons_verification_id",
                table: "etalons",
                column: "verification_id");

            migrationBuilder.CreateIndex(
                name: "ix_verifications_device_id",
                table: "verifications",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_verifications_device_type_id",
                table: "verifications",
                column: "device_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "etalons");

            migrationBuilder.DropTable(
                name: "verifications");

            migrationBuilder.DropTable(
                name: "device_types");

            migrationBuilder.DropTable(
                name: "devices");
        }
    }
}
