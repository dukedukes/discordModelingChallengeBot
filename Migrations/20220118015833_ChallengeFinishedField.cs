using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModelChallengeBot.Migrations
{
    public partial class ChallengeFinishedField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ChallengeFinished",
                table: "ModelingChallenge",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChallengeFinished",
                table: "ModelingChallenge");
        }
    }
}
