using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModelChallengeBot.Migrations
{
    public partial class Submissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Submission",
                columns: table => new
                {
                    SubmissionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Submitter = table.Column<ulong>(type: "INTEGER", nullable: false),
                    TimeTaken = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: true),
                    ModelingChallengeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submission", x => x.SubmissionId);
                    table.ForeignKey(
                        name: "FK_Submission_ModelingChallenge_ModelingChallengeId",
                        column: x => x.ModelingChallengeId,
                        principalTable: "ModelingChallenge",
                        principalColumn: "ModelingChallengeId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Submission_ModelingChallengeId",
                table: "Submission",
                column: "ModelingChallengeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Submission");
        }
    }
}
