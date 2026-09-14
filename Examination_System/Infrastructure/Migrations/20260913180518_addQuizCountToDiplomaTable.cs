using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addQuizCountToDiplomaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuizCount",
                table: "Diplomas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StudentDiplomas",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiplomaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDiplomas", x => new { x.StudentId, x.DiplomaId });
                    table.ForeignKey(
                        name: "FK_StudentDiplomas_Diplomas_DiplomaId",
                        column: x => x.DiplomaId,
                        principalTable: "Diplomas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Diplomas",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "QuizCount",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Diplomas",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "QuizCount",
                value: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentDiplomas_DiplomaId",
                table: "StudentDiplomas",
                column: "DiplomaId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDiplomas_StudentId",
                table: "StudentDiplomas",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentDiplomas");

            migrationBuilder.DropColumn(
                name: "QuizCount",
                table: "Diplomas");
        }
    }
}
