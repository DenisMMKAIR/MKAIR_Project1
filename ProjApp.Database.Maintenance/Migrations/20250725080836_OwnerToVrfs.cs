using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjApp.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class OwnerToVrfs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "owner",
                table: "manometr1verifications",
                newName: "owner_initial_name");

            migrationBuilder.RenameColumn(
                name: "owner",
                table: "davlenie1verifications",
                newName: "owner_initial_name");

            migrationBuilder.AddColumn<Guid>(
                name: "owner_id",
                table: "manometr1verifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "owner_id",
                table: "davlenie1verifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_manometr1verifications_owner_id",
                table: "manometr1verifications",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_davlenie1verifications_owner_id",
                table: "davlenie1verifications",
                column: "owner_id");

            migrationBuilder.AddForeignKey(
                name: "fk_davlenie1verifications_owners_owner_id",
                table: "davlenie1verifications",
                column: "owner_id",
                principalTable: "owners",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_manometr1verifications_owners_owner_id",
                table: "manometr1verifications",
                column: "owner_id",
                principalTable: "owners",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_davlenie1verifications_owners_owner_id",
                table: "davlenie1verifications");

            migrationBuilder.DropForeignKey(
                name: "fk_manometr1verifications_owners_owner_id",
                table: "manometr1verifications");

            migrationBuilder.DropIndex(
                name: "ix_manometr1verifications_owner_id",
                table: "manometr1verifications");

            migrationBuilder.DropIndex(
                name: "ix_davlenie1verifications_owner_id",
                table: "davlenie1verifications");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "manometr1verifications");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "davlenie1verifications");

            migrationBuilder.RenameColumn(
                name: "owner_initial_name",
                table: "manometr1verifications",
                newName: "owner");

            migrationBuilder.RenameColumn(
                name: "owner_initial_name",
                table: "davlenie1verifications",
                newName: "owner");
        }
    }
}
