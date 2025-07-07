using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjApp.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeINNType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "inn",
                table: "owners",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<decimal>(
                name: "owner_inn",
                table: "initial_verification_failed",
                type: "numeric(20,0)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "owner_inn",
                table: "initial_verification",
                type: "numeric(20,0)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "inn",
                table: "owners",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<long>(
                name: "owner_inn",
                table: "initial_verification_failed",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "owner_inn",
                table: "initial_verification",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldNullable: true);
        }
    }
}
