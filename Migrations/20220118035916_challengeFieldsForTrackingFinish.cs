using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModelChallengeBot.Migrations
{
    public partial class challengeFieldsForTrackingFinish : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "ChallengeFinishedThreadId",
                table: "ModelingChallenge",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChallengeFinishedThreadId",
                table: "ModelingChallenge");
        }
    }
}
