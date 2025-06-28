using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.FGIS.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class MergeEtalon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_etalon_verification_docs_etalons_etalon_number",
                table: "etalon_verification_docs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_etalon_verification_docs",
                table: "etalon_verification_docs");

            migrationBuilder.DropIndex(
                name: "ix_etalon_verification_docs_etalon_number",
                table: "etalon_verification_docs");

            migrationBuilder.AlterColumn<string>(
                name: "etalon_number",
                table: "etalon_verification_docs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "etalon_verification_docs",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_etalon_verification_docs",
                table: "etalon_verification_docs",
                columns: new[] { "etalon_number", "id" });

            migrationBuilder.AddForeignKey(
                name: "fk_etalon_verification_docs_etalons_etalon_number",
                table: "etalon_verification_docs",
                column: "etalon_number",
                principalTable: "etalons",
                principalColumn: "number",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_etalon_verification_docs_etalons_etalon_number",
                table: "etalon_verification_docs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_etalon_verification_docs",
                table: "etalon_verification_docs");

            migrationBuilder.DropColumn(
                name: "id",
                table: "etalon_verification_docs");

            migrationBuilder.AlterColumn<string>(
                name: "etalon_number",
                table: "etalon_verification_docs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "pk_etalon_verification_docs",
                table: "etalon_verification_docs",
                column: "vri_id");

            migrationBuilder.CreateIndex(
                name: "ix_etalon_verification_docs_etalon_number",
                table: "etalon_verification_docs",
                column: "etalon_number");

            migrationBuilder.AddForeignKey(
                name: "fk_etalon_verification_docs_etalons_etalon_number",
                table: "etalon_verification_docs",
                column: "etalon_number",
                principalTable: "etalons",
                principalColumn: "number");
        }
    }
}
