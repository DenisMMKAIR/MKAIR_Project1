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
                    notation = table.Column<string>(type: "text", nullable: false)
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
                name: "pending_manometr_verifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_type_number = table.Column<string>(type: "text", nullable: false),
                    device_serial = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    verification_methods = table.Column<string[]>(type: "text[]", nullable: false),
                    etalons_numbers = table.Column<string[]>(type: "text[]", nullable: false),
                    owner_name = table.Column<string>(type: "text", nullable: false),
                    worker_name = table.Column<string>(type: "text", nullable: false),
                    temperature = table.Column<double>(type: "double precision", nullable: false),
                    pressure = table.Column<string>(type: "text", nullable: false),
                    hummidity = table.Column<double>(type: "double precision", nullable: false),
                    accuracy = table.Column<double>(type: "double precision", nullable: true),
                    location = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pending_manometr_verifications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "protocol_templates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_type_numbers = table.Column<string[]>(type: "text[]", nullable: false),
                    group = table.Column<string>(type: "text", nullable: false),
                    checkups = table.Column<string>(type: "text", nullable: false),
                    values = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_protocol_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "verification_methods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    aliases = table.Column<string[]>(type: "text[]", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verification_methods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    serial = table.Column<string>(type: "text", nullable: false),
                    manufactured_year = table.Column<long>(type: "bigint", nullable: false),
                    device_type_number = table.Column<string>(type: "text", nullable: false),
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
                name: "protocol_template_verification_method",
                columns: table => new
                {
                    protocol_templates_id = table.Column<Guid>(type: "uuid", nullable: false),
                    verification_methods_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_protocol_template_verification_method", x => new { x.protocol_templates_id, x.verification_methods_id });
                    table.ForeignKey(
                        name: "fk_protocol_template_verification_method_protocol_templates_pr",
                        column: x => x.protocol_templates_id,
                        principalTable: "protocol_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_protocol_template_verification_method_verification_methods_",
                        column: x => x.verification_methods_id,
                        principalTable: "verification_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "initial_verification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_type_number = table.Column<string>(type: "text", nullable: false),
                    device_serial = table.Column<string>(type: "text", nullable: false),
                    owner = table.Column<string>(type: "text", nullable: false),
                    verification_type_names = table.Column<string[]>(type: "text[]", nullable: false),
                    verification_date = table.Column<DateOnly>(type: "date", nullable: false),
                    verified_until_date = table.Column<DateOnly>(type: "date", nullable: false),
                    additional_info = table.Column<string>(type: "text", nullable: true),
                    device_id = table.Column<Guid>(type: "uuid", nullable: true),
                    protocol_template_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_initial_verification", x => x.id);
                    table.ForeignKey(
                        name: "fk_initial_verification_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_initial_verification_protocol_templates_protocol_template_id",
                        column: x => x.protocol_template_id,
                        principalTable: "protocol_templates",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "initial_verification_failed",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_type_number = table.Column<string>(type: "text", nullable: false),
                    device_serial = table.Column<string>(type: "text", nullable: false),
                    owner = table.Column<string>(type: "text", nullable: false),
                    verification_type_names = table.Column<string[]>(type: "text[]", nullable: false),
                    verification_date = table.Column<DateOnly>(type: "date", nullable: false),
                    failed_doc_number = table.Column<string>(type: "text", nullable: false),
                    additional_info = table.Column<string>(type: "text", nullable: true),
                    device_id = table.Column<Guid>(type: "uuid", nullable: true),
                    protocol_template_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_initial_verification_failed", x => x.id);
                    table.ForeignKey(
                        name: "fk_initial_verification_failed_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_initial_verification_failed_protocol_templates_protocol_tem",
                        column: x => x.protocol_template_id,
                        principalTable: "protocol_templates",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "etalon_initial_verification",
                columns: table => new
                {
                    etalons_id = table.Column<Guid>(type: "uuid", nullable: false),
                    initial_verifications_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_etalon_initial_verification", x => new { x.etalons_id, x.initial_verifications_id });
                    table.ForeignKey(
                        name: "fk_etalon_initial_verification_etalons_etalons_id",
                        column: x => x.etalons_id,
                        principalTable: "etalons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_etalon_initial_verification_initial_verification_initial_ve",
                        column: x => x.initial_verifications_id,
                        principalTable: "initial_verification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "etalon_initial_verification_failed",
                columns: table => new
                {
                    etalons_id = table.Column<Guid>(type: "uuid", nullable: false),
                    initial_verifications_failed_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_etalon_initial_verification_failed", x => new { x.etalons_id, x.initial_verifications_failed_id });
                    table.ForeignKey(
                        name: "fk_etalon_initial_verification_failed_etalons_etalons_id",
                        column: x => x.etalons_id,
                        principalTable: "etalons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_etalon_initial_verification_failed_initial_verification_fai",
                        column: x => x.initial_verifications_failed_id,
                        principalTable: "initial_verification_failed",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_devices_device_type_id",
                table: "devices",
                column: "device_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_etalon_initial_verification_initial_verifications_id",
                table: "etalon_initial_verification",
                column: "initial_verifications_id");

            migrationBuilder.CreateIndex(
                name: "ix_etalon_initial_verification_failed_initial_verifications_fa",
                table: "etalon_initial_verification_failed",
                column: "initial_verifications_failed_id");

            migrationBuilder.CreateIndex(
                name: "ix_initial_verification_device_id",
                table: "initial_verification",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_initial_verification_protocol_template_id",
                table: "initial_verification",
                column: "protocol_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_initial_verification_failed_device_id",
                table: "initial_verification_failed",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_initial_verification_failed_protocol_template_id",
                table: "initial_verification_failed",
                column: "protocol_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_protocol_template_verification_method_verification_methods_",
                table: "protocol_template_verification_method",
                column: "verification_methods_id");

            migrationBuilder.CreateIndex(
                name: "ix_verification_method_files_verification_method_id",
                table: "verification_method_files",
                column: "verification_method_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "etalon_initial_verification");

            migrationBuilder.DropTable(
                name: "etalon_initial_verification_failed");

            migrationBuilder.DropTable(
                name: "initial_verification_jobs");

            migrationBuilder.DropTable(
                name: "pending_manometr_verifications");

            migrationBuilder.DropTable(
                name: "protocol_template_verification_method");

            migrationBuilder.DropTable(
                name: "verification_method_files");

            migrationBuilder.DropTable(
                name: "initial_verification");

            migrationBuilder.DropTable(
                name: "etalons");

            migrationBuilder.DropTable(
                name: "initial_verification_failed");

            migrationBuilder.DropTable(
                name: "verification_methods");

            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "protocol_templates");

            migrationBuilder.DropTable(
                name: "device_types");
        }
    }
}
