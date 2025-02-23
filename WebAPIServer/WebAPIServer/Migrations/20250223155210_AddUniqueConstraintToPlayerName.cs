using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIServer.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToPlayerName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PlayerName",
                table: "player_data",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_player_data_PlayerName",
                table: "player_data",
                column: "PlayerName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_player_data_PlayerName",
                table: "player_data");

            migrationBuilder.AlterColumn<string>(
                name: "PlayerName",
                table: "player_data",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
