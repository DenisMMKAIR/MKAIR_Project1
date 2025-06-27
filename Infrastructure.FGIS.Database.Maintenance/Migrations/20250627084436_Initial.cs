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
                name: "etalons",
                columns: table => new
                {
                    number = table.Column<string>(type: "text", nullable: false),
                    mi_type_num = table.Column<string>(type: "text", nullable: false),
                    mi_type = table.Column<string>(type: "text", nullable: false),
                    mi_notation = table.Column<string>(type: "text", nullable: false),
                    modification = table.Column<string>(type: "text", nullable: false),
                    factory_num = table.Column<string>(type: "text", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    schematype = table.Column<string>(type: "text", nullable: false),
                    schematitle = table.Column<string>(type: "text", nullable: false),
                    np_enumber = table.Column<string>(type: "text", nullable: false),
                    rank_code = table.Column<string>(type: "text", nullable: false),
                    rank_class = table.Column<string>(type: "text", nullable: false),
                    applicability = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_etalons", x => x.number);
                });

            migrationBuilder.CreateTable(
                name: "month_results",
                columns: table => new
                {
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    done = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_month_results", x => x.date);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_serial_number = table.Column<string>(type: "text", nullable: false),
                    device_manufactured_year = table.Column<long>(type: "bigint", nullable: false),
                    device_modification = table.Column<string>(type: "text", nullable: false),
                    device_type_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_devices", x => x.id);
                    table.ForeignKey(
                        name: "fk_devices_device_types_device_type_id",
                        column: x => x.device_type_id,
                        principalTable: "device_types",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "etalon_verification_docs",
                columns: table => new
                {
                    vri_id = table.Column<string>(type: "text", nullable: false),
                    org_title = table.Column<string>(type: "text", nullable: false),
                    verification_date = table.Column<string>(type: "text", nullable: false),
                    valid_date = table.Column<string>(type: "text", nullable: false),
                    result_docnum = table.Column<string>(type: "text", nullable: false),
                    applicability = table.Column<bool>(type: "boolean", nullable: false),
                    etalon_number = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_etalon_verification_docs", x => x.vri_id);
                    table.ForeignKey(
                        name: "fk_etalon_verification_docs_etalons_etalon_number",
                        column: x => x.etalon_number,
                        principalTable: "etalons",
                        principalColumn: "number");
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
                    etalon_numbers = table.Column<string[]>(type: "text[]", nullable: false),
                    device_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_verifications_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_devices_device_type_id",
                table: "devices",
                column: "device_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_etalon_verification_docs_etalon_number",
                table: "etalon_verification_docs",
                column: "etalon_number");

            migrationBuilder.CreateIndex(
                name: "ix_verifications_device_id",
                table: "verifications",
                column: "device_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "etalon_verification_docs");

            migrationBuilder.DropTable(
                name: "month_results");

            migrationBuilder.DropTable(
                name: "verifications");

            migrationBuilder.DropTable(
                name: "etalons");

            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "device_types");
        }
    }
}
