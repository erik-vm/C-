using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameStates",
                columns: table => new
                {
                    GameId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Player1Name = table.Column<string>(type: "TEXT", nullable: false),
                    Player1Color = table.Column<string>(type: "TEXT", nullable: false),
                    Player1IsAi = table.Column<bool>(type: "INTEGER", nullable: false),
                    Player1AiDifficulty = table.Column<string>(type: "TEXT", nullable: true),
                    Player2Name = table.Column<string>(type: "TEXT", nullable: false),
                    Player2Color = table.Column<string>(type: "TEXT", nullable: false),
                    Player2IsAi = table.Column<bool>(type: "INTEGER", nullable: false),
                    Player2AiDifficulty = table.Column<string>(type: "TEXT", nullable: true),
                    BoardHeight = table.Column<int>(type: "INTEGER", nullable: false),
                    BoardWidth = table.Column<int>(type: "INTEGER", nullable: false),
                    BoardShape = table.Column<string>(type: "TEXT", nullable: false),
                    WinningConnection = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentPlayerName = table.Column<string>(type: "TEXT", nullable: false),
                    IsGameOver = table.Column<bool>(type: "INTEGER", nullable: false),
                    WinnerName = table.Column<string>(type: "TEXT", nullable: true),
                    GameMode = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStates", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "BoardCells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameStateId = table.Column<string>(type: "TEXT", nullable: false),
                    Row = table.Column<int>(type: "INTEGER", nullable: false),
                    Column = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardCells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardCells_GameStates_GameStateId",
                        column: x => x.GameStateId,
                        principalTable: "GameStates",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardCells_GameStateId",
                table: "BoardCells",
                column: "GameStateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardCells");

            migrationBuilder.DropTable(
                name: "GameStates");
        }
    }
}
