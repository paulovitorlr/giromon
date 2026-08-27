using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Giromon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGameRounds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game_rounds",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bet_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    first_symbol = table.Column<int>(type: "integer", nullable: false),
                    second_symbol = table.Column<int>(type: "integer", nullable: false),
                    third_symbol = table.Column<int>(type: "integer", nullable: false),
                    prize_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_rounds", x => x.id);
                    table.ForeignKey(
                        name: "FK_game_rounds_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_game_rounds_user_id",
                table: "game_rounds",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_rounds");
        }
    }
}
