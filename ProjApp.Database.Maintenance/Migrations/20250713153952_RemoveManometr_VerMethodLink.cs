using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjApp.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class RemoveManometr_VerMethodLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_manometr1verifications_verification_methods_verification_me",
                table: "manometr1verifications");

            migrationBuilder.DropIndex(
                name: "ix_manometr1verifications_verification_method_id",
                table: "manometr1verifications");

            migrationBuilder.DropColumn(
                name: "verification_method_id",
                table: "manometr1verifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "verification_method_id",
                table: "manometr1verifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_manometr1verifications_verification_method_id",
                table: "manometr1verifications",
                column: "verification_method_id");

            migrationBuilder.AddForeignKey(
                name: "fk_manometr1verifications_verification_methods_verification_me",
                table: "manometr1verifications",
                column: "verification_method_id",
                principalTable: "verification_methods",
                principalColumn: "id");
        }
    }
}
