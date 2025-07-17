using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.FGIS.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "device_type_ids_collected",
                table: "month_results",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "device_types_collected",
                table: "month_results",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "device_type_ids",
                columns: table => new
                {
                    mit_uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_type_ids", x => x.mit_uuid);
                });

            migrationBuilder.CreateTable(
                name: "device_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    notation = table.Column<string[]>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "manufacturer_class",
                columns: table => new
                {
                    device_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_manufacturer_class", x => new { x.device_type_id, x.id });
                    table.ForeignKey(
                        name: "fk_manufacturer_class_device_types_device_type_id",
                        column: x => x.device_type_id,
                        principalTable: "device_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "method_class",
                columns: table => new
                {
                    device_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false),
                    doc_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_method_class", x => new { x.device_type_id, x.id });
                    table.ForeignKey(
                        name: "fk_method_class_device_types_device_type_id",
                        column: x => x.device_type_id,
                        principalTable: "device_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "spec_class",
                columns: table => new
                {
                    device_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    doc_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spec_class", x => new { x.device_type_id, x.id });
                    table.ForeignKey(
                        name: "fk_spec_class_device_types_device_type_id",
                        column: x => x.device_type_id,
                        principalTable: "device_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_device_type_ids_number",
                table: "device_type_ids",
                column: "number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_device_types_number",
                table: "device_types",
                column: "number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "device_type_ids");

            migrationBuilder.DropTable(
                name: "manufacturer_class");

            migrationBuilder.DropTable(
                name: "method_class");

            migrationBuilder.DropTable(
                name: "spec_class");

            migrationBuilder.DropTable(
                name: "device_types");

            migrationBuilder.DropColumn(
                name: "device_type_ids_collected",
                table: "month_results");

            migrationBuilder.DropColumn(
                name: "device_types_collected",
                table: "month_results");
        }
    }
}
