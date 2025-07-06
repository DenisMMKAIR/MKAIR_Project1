using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjApp.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddIVColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "humidity",
                table: "initial_verification_failed",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "location",
                table: "initial_verification_failed",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "owner_inn",
                table: "initial_verification_failed",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pressure",
                table: "initial_verification_failed",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "temperature",
                table: "initial_verification_failed",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "verification_type_num",
                table: "initial_verification_failed",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "worker",
                table: "initial_verification_failed",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "humidity",
                table: "initial_verification",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "location",
                table: "initial_verification",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "owner_inn",
                table: "initial_verification",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pressure",
                table: "initial_verification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "temperature",
                table: "initial_verification",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "verification_type_num",
                table: "initial_verification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "worker",
                table: "initial_verification",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "humidity",
                table: "initial_verification_failed");

            migrationBuilder.DropColumn(
                name: "location",
                table: "initial_verification_failed");

            migrationBuilder.DropColumn(
                name: "owner_inn",
                table: "initial_verification_failed");

            migrationBuilder.DropColumn(
                name: "pressure",
                table: "initial_verification_failed");

            migrationBuilder.DropColumn(
                name: "temperature",
                table: "initial_verification_failed");

            migrationBuilder.DropColumn(
                name: "verification_type_num",
                table: "initial_verification_failed");

            migrationBuilder.DropColumn(
                name: "worker",
                table: "initial_verification_failed");

            migrationBuilder.DropColumn(
                name: "humidity",
                table: "initial_verification");

            migrationBuilder.DropColumn(
                name: "location",
                table: "initial_verification");

            migrationBuilder.DropColumn(
                name: "owner_inn",
                table: "initial_verification");

            migrationBuilder.DropColumn(
                name: "pressure",
                table: "initial_verification");

            migrationBuilder.DropColumn(
                name: "temperature",
                table: "initial_verification");

            migrationBuilder.DropColumn(
                name: "verification_type_num",
                table: "initial_verification");

            migrationBuilder.DropColumn(
                name: "worker",
                table: "initial_verification");
        }
    }
}
