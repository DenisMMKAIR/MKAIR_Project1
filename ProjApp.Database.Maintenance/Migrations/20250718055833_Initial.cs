using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjApp.Database.Maintenance.Migrations
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
                    number = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    notation = table.Column<string>(type: "text", nullable: false),
                    method_urls = table.Column<string[]>(type: "text[]", nullable: true),
                    spec_urls = table.Column<string[]>(type: "text[]", nullable: true),
                    manufacturers = table.Column<string[]>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "etalons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    to_date = table.Column<DateOnly>(type: "date", nullable: false),
                    full_info = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_etalons", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "initial_verification_jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_initial_verification_jobs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "owners",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    inn = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_owners", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "protocol_templates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    verification_group = table.Column<int>(type: "integer", nullable: false),
                    protocol_group = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_protocol_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_type_number = table.Column<string>(type: "text", nullable: false),
                    serial = table.Column<string>(type: "text", nullable: false),
                    manufactured_year = table.Column<long>(type: "bigint", nullable: false),
                    modification = table.Column<string>(type: "text", nullable: false),
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
                name: "verification_methods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    aliases = table.Column<string[]>(type: "text[]", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    checkups = table.Column<string>(type: "text", nullable: false),
                    protocol_template_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verification_methods", x => x.id);
                    table.ForeignKey(
                        name: "fk_verification_methods_protocol_templates_protocol_template_id",
                        column: x => x.protocol_template_id,
                        principalTable: "protocol_templates",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "failed_initial_verifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_type_number = table.Column<string>(type: "text", nullable: false),
                    device_serial = table.Column<string>(type: "text", nullable: false),
                    owner = table.Column<string>(type: "text", nullable: false),
                    verification_type_name = table.Column<string>(type: "text", nullable: false),
                    verification_date = table.Column<DateOnly>(type: "date", nullable: false),
                    failed_doc_number = table.Column<string>(type: "text", nullable: false),
                    verification_group = table.Column<int>(type: "integer", nullable: true),
                    protocol_number = table.Column<string>(type: "text", nullable: true),
                    owner_inn = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    worker = table.Column<string>(type: "text", nullable: true),
                    location = table.Column<int>(type: "integer", nullable: true),
                    pressure = table.Column<string>(type: "text", nullable: true),
                    temperature = table.Column<double>(type: "double precision", nullable: true),
                    humidity = table.Column<double>(type: "double precision", nullable: true),
                    measurement_min = table.Column<double>(type: "double precision", nullable: true),
                    measurement_max = table.Column<double>(type: "double precision", nullable: true),
                    measurement_unit = table.Column<string>(type: "text", nullable: true),
                    accuracy = table.Column<double>(type: "double precision", nullable: true),
                    device_id = table.Column<Guid>(type: "uuid", nullable: true),
                    verification_method_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_failed_initial_verifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_failed_initial_verifications_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_failed_initial_verifications_verification_methods_verificat",
                        column: x => x.verification_method_id,
                        principalTable: "verification_methods",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "manometr1verifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    protocol_number = table.Column<string>(type: "text", nullable: false),
                    device_type_name = table.Column<string>(type: "text", nullable: false),
                    device_modification = table.Column<string>(type: "text", nullable: false),
                    device_type_number = table.Column<string>(type: "text", nullable: false),
                    device_serial = table.Column<string>(type: "text", nullable: false),
                    manufacture_year = table.Column<long>(type: "bigint", nullable: false),
                    owner = table.Column<string>(type: "text", nullable: false),
                    owner_inn = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    verifications_info = table.Column<string>(type: "text", nullable: false),
                    etalons_info = table.Column<string>(type: "text", nullable: false),
                    temperature = table.Column<double>(type: "double precision", nullable: false),
                    humidity = table.Column<double>(type: "double precision", nullable: false),
                    pressure = table.Column<string>(type: "text", nullable: false),
                    verification_visual_checkup = table.Column<string>(type: "text", nullable: false),
                    verification_result_checkup = table.Column<string>(type: "text", nullable: false),
                    verification_accuracy_checkup = table.Column<string>(type: "text", nullable: false),
                    verification_date = table.Column<DateOnly>(type: "date", nullable: false),
                    worker = table.Column<string>(type: "text", nullable: false),
                    verification_group = table.Column<int>(type: "integer", nullable: false),
                    location = table.Column<int>(type: "integer", nullable: false),
                    verified_until_date = table.Column<DateOnly>(type: "date", nullable: false),
                    measurement_min = table.Column<double>(type: "double precision", nullable: false),
                    measurement_max = table.Column<double>(type: "double precision", nullable: false),
                    measurement_unit = table.Column<string>(type: "text", nullable: false),
                    valid_error = table.Column<double>(type: "double precision", nullable: false),
                    device_values = table.Column<string>(type: "text", nullable: false),
                    etalon_values = table.Column<string>(type: "text", nullable: false),
                    actual_error = table.Column<string>(type: "text", nullable: false),
                    actual_variation = table.Column<double[]>(type: "double precision[]", nullable: false),
                    device_id = table.Column<Guid>(type: "uuid", nullable: true),
                    verification_method_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_manometr1verifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_manometr1verifications_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_manometr1verifications_verification_methods_verification_me",
                        column: x => x.verification_method_id,
                        principalTable: "verification_methods",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "success_initial_verifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_type_number = table.Column<string>(type: "text", nullable: false),
                    device_serial = table.Column<string>(type: "text", nullable: false),
                    owner = table.Column<string>(type: "text", nullable: false),
                    verification_type_name = table.Column<string>(type: "text", nullable: false),
                    verification_date = table.Column<DateOnly>(type: "date", nullable: false),
                    verified_until_date = table.Column<DateOnly>(type: "date", nullable: false),
                    verification_group = table.Column<int>(type: "integer", nullable: true),
                    protocol_number = table.Column<string>(type: "text", nullable: true),
                    owner_inn = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    worker = table.Column<string>(type: "text", nullable: true),
                    location = table.Column<int>(type: "integer", nullable: true),
                    pressure = table.Column<string>(type: "text", nullable: true),
                    temperature = table.Column<double>(type: "double precision", nullable: true),
                    humidity = table.Column<double>(type: "double precision", nullable: true),
                    measurement_min = table.Column<double>(type: "double precision", nullable: true),
                    measurement_max = table.Column<double>(type: "double precision", nullable: true),
                    measurement_unit = table.Column<string>(type: "text", nullable: true),
                    accuracy = table.Column<double>(type: "double precision", nullable: true),
                    device_id = table.Column<Guid>(type: "uuid", nullable: true),
                    verification_method_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_success_initial_verifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_success_initial_verifications_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_success_initial_verifications_verification_methods_verifica",
                        column: x => x.verification_method_id,
                        principalTable: "verification_methods",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "verification_method_files",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    mimetype = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<byte[]>(type: "bytea", nullable: false),
                    verification_method_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verification_method_files", x => x.id);
                    table.ForeignKey(
                        name: "fk_verification_method_files_verification_methods_verification",
                        column: x => x.verification_method_id,
                        principalTable: "verification_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "etalon_failed_initial_verification",
                columns: table => new
                {
                    etalons_id = table.Column<Guid>(type: "uuid", nullable: false),
                    failed_initial_verifications_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_etalon_failed_initial_verification", x => new { x.etalons_id, x.failed_initial_verifications_id });
                    table.ForeignKey(
                        name: "fk_etalon_failed_initial_verification_etalons_etalons_id",
                        column: x => x.etalons_id,
                        principalTable: "etalons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_etalon_failed_initial_verification_failed_initial_verificat",
                        column: x => x.failed_initial_verifications_id,
                        principalTable: "failed_initial_verifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "etalon_manometr1verification",
                columns: table => new
                {
                    etalons_id = table.Column<Guid>(type: "uuid", nullable: false),
                    manometr1verifications_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_etalon_manometr1verification", x => new { x.etalons_id, x.manometr1verifications_id });
                    table.ForeignKey(
                        name: "fk_etalon_manometr1verification_etalons_etalons_id",
                        column: x => x.etalons_id,
                        principalTable: "etalons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_etalon_manometr1verification_manometr1verifications_manomet",
                        column: x => x.manometr1verifications_id,
                        principalTable: "manometr1verifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "etalon_success_initial_verification",
                columns: table => new
                {
                    etalons_id = table.Column<Guid>(type: "uuid", nullable: false),
                    success_initial_verifications_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_etalon_success_initial_verification", x => new { x.etalons_id, x.success_initial_verifications_id });
                    table.ForeignKey(
                        name: "fk_etalon_success_initial_verification_etalons_etalons_id",
                        column: x => x.etalons_id,
                        principalTable: "etalons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_etalon_success_initial_verification_success_initial_verific",
                        column: x => x.success_initial_verifications_id,
                        principalTable: "success_initial_verifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_devices_device_type_id",
                table: "devices",
                column: "device_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_etalon_failed_initial_verification_failed_initial_verificat",
                table: "etalon_failed_initial_verification",
                column: "failed_initial_verifications_id");

            migrationBuilder.CreateIndex(
                name: "ix_etalon_manometr1verification_manometr1verifications_id",
                table: "etalon_manometr1verification",
                column: "manometr1verifications_id");

            migrationBuilder.CreateIndex(
                name: "ix_etalon_success_initial_verification_success_initial_verific",
                table: "etalon_success_initial_verification",
                column: "success_initial_verifications_id");

            migrationBuilder.CreateIndex(
                name: "ix_failed_initial_verifications_device_id",
                table: "failed_initial_verifications",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_failed_initial_verifications_verification_method_id",
                table: "failed_initial_verifications",
                column: "verification_method_id");

            migrationBuilder.CreateIndex(
                name: "ix_manometr1verifications_device_id",
                table: "manometr1verifications",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_manometr1verifications_verification_method_id",
                table: "manometr1verifications",
                column: "verification_method_id");

            migrationBuilder.CreateIndex(
                name: "ix_success_initial_verifications_device_id",
                table: "success_initial_verifications",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_success_initial_verifications_verification_method_id",
                table: "success_initial_verifications",
                column: "verification_method_id");

            migrationBuilder.CreateIndex(
                name: "ix_verification_method_files_verification_method_id",
                table: "verification_method_files",
                column: "verification_method_id");

            migrationBuilder.CreateIndex(
                name: "ix_verification_methods_protocol_template_id",
                table: "verification_methods",
                column: "protocol_template_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "etalon_failed_initial_verification");

            migrationBuilder.DropTable(
                name: "etalon_manometr1verification");

            migrationBuilder.DropTable(
                name: "etalon_success_initial_verification");

            migrationBuilder.DropTable(
                name: "initial_verification_jobs");

            migrationBuilder.DropTable(
                name: "owners");

            migrationBuilder.DropTable(
                name: "verification_method_files");

            migrationBuilder.DropTable(
                name: "failed_initial_verifications");

            migrationBuilder.DropTable(
                name: "manometr1verifications");

            migrationBuilder.DropTable(
                name: "etalons");

            migrationBuilder.DropTable(
                name: "success_initial_verifications");

            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "verification_methods");

            migrationBuilder.DropTable(
                name: "device_types");

            migrationBuilder.DropTable(
                name: "protocol_templates");
        }
    }
}
