using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModelChallengeBot.Migrations
{
    public partial class AddedChallengeAndImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelingChallenge",
                columns: table => new
                {
                    ModelingChallengeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChallengeName = table.Column<string>(type: "TEXT", nullable: true),
                    ChallengeDescription = table.Column<string>(type: "TEXT", nullable: true),
                    ChallengeDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    ListingId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelingChallenge", x => x.ModelingChallengeId);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeImage",
                columns: table => new
                {
                    ChallengeImageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    ModelingChallengeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeImage", x => x.ChallengeImageId);
                    table.ForeignKey(
                        name: "FK_ChallengeImage_ModelingChallenge_ModelingChallengeId",
                        column: x => x.ModelingChallengeId,
                        principalTable: "ModelingChallenge",
                        principalColumn: "ModelingChallengeId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeImage_ModelingChallengeId",
                table: "ChallengeImage",
                column: "ModelingChallengeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChallengeImage");

            migrationBuilder.DropTable(
                name: "ModelingChallenge");
        }
    }
}
