using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.FGIS.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class EtalonIdKey_Id_Date : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_etalon_ids",
                table: "etalon_ids");

            migrationBuilder.AddPrimaryKey(
                name: "pk_etalon_ids",
                table: "etalon_ids",
                columns: new[] { "rmieta_id", "date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_etalon_ids",
                table: "etalon_ids");

            migrationBuilder.AddPrimaryKey(
                name: "pk_etalon_ids",
                table: "etalon_ids",
                column: "rmieta_id");
        }
    }
}
