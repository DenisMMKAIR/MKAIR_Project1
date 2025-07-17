using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.FGIS.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMonthResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "done",
                table: "month_results");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "done",
                table: "month_results",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
