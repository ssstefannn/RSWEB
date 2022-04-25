using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RSWEB.Migrations
{
    public partial class v0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<string>(maxLength: 10, nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    EnrollmentDate = table.Column<DateTime>(nullable: false),
                    AcquiredCredits = table.Column<int>(nullable: false),
                    CurrentSemester = table.Column<int>(nullable: false),
                    EducationLevel = table.Column<string>(maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    Degree = table.Column<string>(maxLength: 50, nullable: true),
                    AcademicRank = table.Column<string>(maxLength: 25, nullable: true),
                    OfficeNumber = table.Column<string>(maxLength: 10, nullable: true),
                    HireDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    Credits = table.Column<int>(nullable: false),
                    Semester = table.Column<int>(nullable: false),
                    Programme = table.Column<string>(maxLength: 100, nullable: true),
                    EducationLevel = table.Column<string>(maxLength: 25, nullable: true),
                    FirstTeacherId = table.Column<int>(nullable: false),
                    SecondTeacherId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Teachers_FirstTeacherId",
                        column: x => x.FirstTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Courses_Teachers_SecondTeacherId",
                        column: x => x.SecondTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(nullable: false),
                    StudentId = table.Column<long>(nullable: false),
                    Semester = table.Column<string>(maxLength: 10, nullable: true),
                    Year = table.Column<int>(nullable: false),
                    Grade = table.Column<int>(nullable: false),
                    SeminalUrl = table.Column<string>(maxLength: 255, nullable: true),
                    ProjectUrl = table.Column<string>(maxLength: 255, nullable: true),
                    ExamPoints = table.Column<int>(nullable: false),
                    SeminalPoints = table.Column<int>(nullable: false),
                    ProjectPoints = table.Column<int>(nullable: false),
                    AdditionalPoints = table.Column<int>(nullable: false),
                    FinishDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_FirstTeacherId",
                table: "Courses",
                column: "FirstTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SecondTeacherId",
                table: "Courses",
                column: "SecondTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Teachers");
        }
    }
}
