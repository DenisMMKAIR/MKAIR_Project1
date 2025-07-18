using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.FGIS.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDeviceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "manufacturer_class");

            migrationBuilder.DropTable(
                name: "method_class");

            migrationBuilder.DropTable(
                name: "spec_class");

            migrationBuilder.AddColumn<string[]>(
                name: "manufacturers",
                table: "device_types",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "meth_urls",
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
                name: "meth_urls",
                table: "device_types");

            migrationBuilder.DropColumn(
                name: "spec_urls",
                table: "device_types");

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
                    doc_url = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false)
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
        }
    }
}
