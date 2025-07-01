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
                name: "protocols",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_type_number = table.Column<string>(type: "text", nullable: false),
                    group = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_protocols", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "verification_method_alias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verification_method_alias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "verification_methods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    file_content = table.Column<byte[]>(type: "bytea", nullable: false)
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
                name: "protocol_checkup",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    protocol_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_protocol_checkup", x => new { x.protocol_id, x.id });
                    table.ForeignKey(
                        name: "fk_protocol_checkup_protocols_protocol_id",
                        column: x => x.protocol_id,
                        principalTable: "protocols",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "protocol_verification_method",
                columns: table => new
                {
                    protocols_id = table.Column<Guid>(type: "uuid", nullable: false),
                    verification_methods_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_protocol_verification_method", x => new { x.protocols_id, x.verification_methods_id });
                    table.ForeignKey(
                        name: "fk_protocol_verification_method_protocols_protocols_id",
                        column: x => x.protocols_id,
                        principalTable: "protocols",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_protocol_verification_method_verification_methods_verificat",
                        column: x => x.verification_methods_id,
                        principalTable: "verification_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "verification_method_verification_method_alias",
                columns: table => new
                {
                    aliases_id = table.Column<Guid>(type: "uuid", nullable: false),
                    verification_methods_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verification_method_verification_method_alias", x => new { x.aliases_id, x.verification_methods_id });
                    table.ForeignKey(
                        name: "fk_verification_method_verification_method_alias_verification_",
                        column: x => x.aliases_id,
                        principalTable: "verification_method_alias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_verification_method_verification_method_alias_verification_1",
                        column: x => x.verification_methods_id,
                        principalTable: "verification_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                    device_id = table.Column<Guid>(type: "uuid", nullable: true),
                    additional_info = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_failed_initial_verifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_failed_initial_verifications_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "initial_verifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_type_number = table.Column<string>(type: "text", nullable: false),
                    device_serial = table.Column<string>(type: "text", nullable: false),
                    owner = table.Column<string>(type: "text", nullable: false),
                    verification_type_name = table.Column<string>(type: "text", nullable: false),
                    verification_date = table.Column<DateOnly>(type: "date", nullable: false),
                    verified_until_date = table.Column<DateOnly>(type: "date", nullable: false),
                    device_id = table.Column<Guid>(type: "uuid", nullable: true),
                    additional_info = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_initial_verifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_initial_verifications_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id");
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
                        name: "fk_etalon_initial_verification_failed_failed_initial_verificat",
                        column: x => x.initial_verifications_failed_id,
                        principalTable: "failed_initial_verifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "fk_etalon_initial_verification_initial_verifications_initial_v",
                        column: x => x.initial_verifications_id,
                        principalTable: "initial_verifications",
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
                name: "ix_failed_initial_verifications_device_id",
                table: "failed_initial_verifications",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_initial_verifications_device_id",
                table: "initial_verifications",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_protocol_verification_method_verification_methods_id",
                table: "protocol_verification_method",
                column: "verification_methods_id");

            migrationBuilder.CreateIndex(
                name: "ix_verification_method_verification_method_alias_verification_",
                table: "verification_method_verification_method_alias",
                column: "verification_methods_id");
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
                name: "protocol_checkup");

            migrationBuilder.DropTable(
                name: "protocol_verification_method");

            migrationBuilder.DropTable(
                name: "verification_method_verification_method_alias");

            migrationBuilder.DropTable(
                name: "initial_verifications");

            migrationBuilder.DropTable(
                name: "etalons");

            migrationBuilder.DropTable(
                name: "failed_initial_verifications");

            migrationBuilder.DropTable(
                name: "protocols");

            migrationBuilder.DropTable(
                name: "verification_method_alias");

            migrationBuilder.DropTable(
                name: "verification_methods");

            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "device_types");
        }
    }
}
