using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjApp.Database.Maintenance.Migrations
{
    /// <inheritdoc />
    public partial class InnerAliasesToVrfMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "verification_method_verification_method_alias");

            migrationBuilder.DropTable(
                name: "verification_method_alias");

            migrationBuilder.AddColumn<string[]>(
                name: "aliases",
                table: "verification_methods",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "aliases",
                table: "verification_methods");

            migrationBuilder.CreateTable(
                name: "verification_method_alias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verification_method_alias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "verification_method_verification_method_alias",
                columns: table => new
                {
                    aliases_id = table.Column<Guid>(type: "uuid", nullable: false),
                    verification_methods_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verification_method_verification_method_alias", x => new { x.aliases_id, x.verification_methods_id });
                    table.ForeignKey(
                        name: "fk_verification_method_verification_method_alias_verification_",
                        column: x => x.aliases_id,
                        principalTable: "verification_method_alias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_verification_method_verification_method_alias_verification_1",
                        column: x => x.verification_methods_id,
                        principalTable: "verification_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_verification_method_verification_method_alias_verification_",
                table: "verification_method_verification_method_alias",
                column: "verification_methods_id");
        }
    }
}
