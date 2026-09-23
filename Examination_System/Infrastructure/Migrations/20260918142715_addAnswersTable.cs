using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAnswersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswer_QuestionOptions_SelectedOptionId",
                table: "QuestionAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswer_Questions_QuestionId",
                table: "QuestionAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswer_QuizAttempts_QuizAttemptId",
                table: "QuestionAnswer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionAnswer",
                table: "QuestionAnswer");

            migrationBuilder.RenameTable(
                name: "QuestionAnswer",
                newName: "QuestionAnswers");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionAnswer_SelectedOptionId",
                table: "QuestionAnswers",
                newName: "IX_QuestionAnswers_SelectedOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionAnswer_QuizAttemptId_QuestionId",
                table: "QuestionAnswers",
                newName: "IX_QuestionAnswers_QuizAttemptId_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionAnswer_QuestionId",
                table: "QuestionAnswers",
                newName: "IX_QuestionAnswers_QuestionId");

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionAnswers",
                table: "QuestionAnswers",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "Points",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "Points",
                value: 1);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_QuestionOptions_SelectedOptionId",
                table: "QuestionAnswers",
                column: "SelectedOptionId",
                principalTable: "QuestionOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_Questions_QuestionId",
                table: "QuestionAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_QuizAttempts_QuizAttemptId",
                table: "QuestionAnswers",
                column: "QuizAttemptId",
                principalTable: "QuizAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_QuestionOptions_SelectedOptionId",
                table: "QuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_Questions_QuestionId",
                table: "QuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_QuizAttempts_QuizAttemptId",
                table: "QuestionAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionAnswers",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "Questions");

            migrationBuilder.RenameTable(
                name: "QuestionAnswers",
                newName: "QuestionAnswer");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionAnswers_SelectedOptionId",
                table: "QuestionAnswer",
                newName: "IX_QuestionAnswer_SelectedOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionAnswers_QuizAttemptId_QuestionId",
                table: "QuestionAnswer",
                newName: "IX_QuestionAnswer_QuizAttemptId_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionAnswers_QuestionId",
                table: "QuestionAnswer",
                newName: "IX_QuestionAnswer_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionAnswer",
                table: "QuestionAnswer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswer_QuestionOptions_SelectedOptionId",
                table: "QuestionAnswer",
                column: "SelectedOptionId",
                principalTable: "QuestionOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswer_Questions_QuestionId",
                table: "QuestionAnswer",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswer_QuizAttempts_QuizAttemptId",
                table: "QuestionAnswer",
                column: "QuizAttemptId",
                principalTable: "QuizAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
