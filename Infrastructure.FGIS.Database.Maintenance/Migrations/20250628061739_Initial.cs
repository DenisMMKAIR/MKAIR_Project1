using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                name: "etalon_ids",
                columns: table => new
                {
                    rmieta_id = table.Column<string>(type: "text", nullable: false),
                    reg_number = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_etalon_ids", x => x.rmieta_id);
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
                    done = table.Column<bool>(type: "boolean", nullable: false),
                    verification_ids_collected = table.Column<bool>(type: "boolean", nullable: false),
                    verifications_collected = table.Column<bool>(type: "boolean", nullable: false),
                    etalons_ids_collected = table.Column<bool>(type: "boolean", nullable: false),
                    etalons_collected = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_month_results", x => x.date);
                });

            migrationBuilder.CreateTable(
                name: "verification_ids",
                columns: table => new
                {
                    vri_id = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verification_ids", x => x.vri_id);
                });

            migrationBuilder.CreateTable(
                name: "verifications",
                columns: table => new
                {
                    vri_id = table.Column<string>(type: "text", nullable: false),
                    mi_info_single_mi_mitype_number = table.Column<string>(type: "text", nullable: false),
                    mi_info_single_mi_mitype_url = table.Column<string>(type: "text", nullable: false),
                    mi_info_single_mi_mitype_type = table.Column<string>(type: "text", nullable: false),
                    mi_info_single_mi_mitype_title = table.Column<string>(type: "text", nullable: false),
                    mi_info_single_mi_manufacture_num = table.Column<string>(type: "text", nullable: false),
                    mi_info_single_mi_manufacture_year = table.Column<int>(type: "integer", nullable: false),
                    mi_info_single_mi_modification = table.Column<string>(type: "text", nullable: false),
                    vri_info_organization = table.Column<string>(type: "text", nullable: false),
                    vri_info_sign_cipher = table.Column<string>(type: "text", nullable: false),
                    vri_info_mi_owner = table.Column<string>(type: "text", nullable: false),
                    vri_info_vrf_date = table.Column<DateOnly>(type: "date", nullable: false),
                    vri_info_valid_date = table.Column<DateOnly>(type: "date", nullable: false),
                    vri_info_vri_type = table.Column<string>(type: "text", nullable: false),
                    vri_info_doc_title = table.Column<string>(type: "text", nullable: false),
                    vri_info_applicable_cert_num = table.Column<string>(type: "text", nullable: false),
                    vri_info_applicable_sign_pass = table.Column<bool>(type: "boolean", nullable: false),
                    vri_info_applicable_sign_mi = table.Column<bool>(type: "boolean", nullable: false),
                    info_brief_indicator = table.Column<bool>(type: "boolean", nullable: false),
                    info_additional_info = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verifications", x => x.vri_id);
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
                name: "Mietum",
                columns: table => new
                {
                    means_class_verification_vri_id = table.Column<string>(type: "text", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reg_number = table.Column<string>(type: "text", nullable: false),
                    mieta_url = table.Column<string>(type: "text", nullable: false),
                    mitype_number = table.Column<string>(type: "text", nullable: false),
                    mitype_url = table.Column<string>(type: "text", nullable: false),
                    mitype_title = table.Column<string>(type: "text", nullable: false),
                    notation = table.Column<string>(type: "text", nullable: false),
                    modification = table.Column<string>(type: "text", nullable: false),
                    manufacture_num = table.Column<string>(type: "text", nullable: false),
                    manufacture_year = table.Column<int>(type: "integer", nullable: false),
                    rank_code = table.Column<string>(type: "text", nullable: false),
                    rank_title = table.Column<string>(type: "text", nullable: false),
                    schema_title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mietum", x => new { x.means_class_verification_vri_id, x.id });
                    table.ForeignKey(
                        name: "fk_mietum_verifications_means_class_verification_vri_id",
                        column: x => x.means_class_verification_vri_id,
                        principalTable: "verifications",
                        principalColumn: "vri_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_etalon_verification_docs_etalon_number",
                table: "etalon_verification_docs",
                column: "etalon_number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "etalon_ids");

            migrationBuilder.DropTable(
                name: "etalon_verification_docs");

            migrationBuilder.DropTable(
                name: "Mietum");

            migrationBuilder.DropTable(
                name: "month_results");

            migrationBuilder.DropTable(
                name: "verification_ids");

            migrationBuilder.DropTable(
                name: "etalons");

            migrationBuilder.DropTable(
                name: "verifications");
        }
    }
}
