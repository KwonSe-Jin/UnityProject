using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebAPIServer.Migrations
{
    /// <inheritdoc />
    public partial class SeedCharacterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "characters",
                columns: new[] { "CharacterId", "Description", "IsOnSale", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "근접 캐릭터", true, "전사", 500 },
                    { 2, "마법 캐릭터", true, "마법사", 600 },
                    { 3, "활 캐릭터", true, "궁수", 550 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "characters",
                keyColumn: "CharacterId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "characters",
                keyColumn: "CharacterId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "characters",
                keyColumn: "CharacterId",
                keyValue: 3);
        }
    }
}
