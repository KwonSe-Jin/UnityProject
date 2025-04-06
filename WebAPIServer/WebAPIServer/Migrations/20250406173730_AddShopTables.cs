using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIServer.Migrations
{
    /// <inheritdoc />
    public partial class AddShopTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    IsOnSale = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characters", x => x.CharacterId);
                });

            migrationBuilder.CreateTable(
                name: "user_characters",
                columns: table => new
                {
                    UserCharacterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    AcquiredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_characters", x => x.UserCharacterId);
                    table.ForeignKey(
                        name: "FK_user_characters_characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_characters_player_data_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "player_data",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_characters_CharacterId",
                table: "user_characters",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_user_characters_PlayerId",
                table: "user_characters",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_characters");

            migrationBuilder.DropTable(
                name: "characters");
        }
    }
}
