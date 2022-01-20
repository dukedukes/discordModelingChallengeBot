using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModelChallengeBot.Migrations
{
    public partial class addChallengeAcceptors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcceptedChallenge",
                columns: table => new
                {
                    AcceptedChallengeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChallengeAcceptor = table.Column<ulong>(type: "INTEGER", nullable: false),
                    AcceptedTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModelingChallengeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptedChallenge", x => x.AcceptedChallengeId);
                    table.ForeignKey(
                        name: "FK_AcceptedChallenge_ModelingChallenge_ModelingChallengeId",
                        column: x => x.ModelingChallengeId,
                        principalTable: "ModelingChallenge",
                        principalColumn: "ModelingChallengeId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedChallenge_ModelingChallengeId",
                table: "AcceptedChallenge",
                column: "ModelingChallengeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcceptedChallenge");
        }
    }
}
