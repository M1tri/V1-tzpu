using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabZakazivanjeAPI.Migrations
{
    /// <inheritdoc />
    public partial class Migracija5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "ActiveVLRs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ActiveVLRs_SessionId",
                table: "ActiveVLRs",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveVLRs_Sessions_SessionId",
                table: "ActiveVLRs",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveVLRs_Sessions_SessionId",
                table: "ActiveVLRs");

            migrationBuilder.DropIndex(
                name: "IX_ActiveVLRs_SessionId",
                table: "ActiveVLRs");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "ActiveVLRs");
        }
    }
}
