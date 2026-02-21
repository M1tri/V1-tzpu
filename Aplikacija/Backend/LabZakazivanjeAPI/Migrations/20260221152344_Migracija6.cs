using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabZakazivanjeAPI.Migrations
{
    /// <inheritdoc />
    public partial class Migracija6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivityClassId",
                table: "Activities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ActivityClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Naziv = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityClasses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ActivityClassId",
                table: "Activities",
                column: "ActivityClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_ActivityClasses_ActivityClassId",
                table: "Activities",
                column: "ActivityClassId",
                principalTable: "ActivityClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_ActivityClasses_ActivityClassId",
                table: "Activities");

            migrationBuilder.DropTable(
                name: "ActivityClasses");

            migrationBuilder.DropIndex(
                name: "IX_Activities_ActivityClassId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "ActivityClassId",
                table: "Activities");
        }
    }
}
