using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjApp.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProtocolTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "protocol_checkup");

            migrationBuilder.DropTable(
                name: "protocol_verification_method");

            migrationBuilder.DropTable(
                name: "protocols");

            migrationBuilder.CreateTable(
                name: "protocol_templates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_type_number = table.Column<string>(type: "text", nullable: false),
                    group = table.Column<string>(type: "text", nullable: false),
                    checkups = table.Column<string>(type: "text", nullable: false),
                    values = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_protocol_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "complete_verification_fails",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    protocol_template_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_complete_verification_fails", x => x.id);
                    table.ForeignKey(
                        name: "fk_complete_verification_fails_protocol_templates_protocol_tem",
                        column: x => x.protocol_template_id,
                        principalTable: "protocol_templates",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "complete_verification_successes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    protocol_template_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_complete_verification_successes", x => x.id);
                    table.ForeignKey(
                        name: "fk_complete_verification_successes_protocol_templates_protocol",
                        column: x => x.protocol_template_id,
                        principalTable: "protocol_templates",
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

            migrationBuilder.CreateIndex(
                name: "ix_complete_verification_fails_protocol_template_id",
                table: "complete_verification_fails",
                column: "protocol_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_complete_verification_successes_protocol_template_id",
                table: "complete_verification_successes",
                column: "protocol_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_protocol_template_verification_method_verification_methods_",
                table: "protocol_template_verification_method",
                column: "verification_methods_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "complete_verification_fails");

            migrationBuilder.DropTable(
                name: "complete_verification_successes");

            migrationBuilder.DropTable(
                name: "protocol_template_verification_method");

            migrationBuilder.DropTable(
                name: "protocol_templates");

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
                name: "protocol_checkup",
                columns: table => new
                {
                    protocol_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "ix_protocol_verification_method_verification_methods_id",
                table: "protocol_verification_method",
                column: "verification_methods_id");
        }
    }
}
