using Microsoft.EntityFrameworkCore.Migrations;

namespace RSWEB.Migrations
{
    public partial class v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "profilePicture",
                table: "Teachers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "profilePicture",
                table: "Students",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "profilePicture",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "profilePicture",
                table: "Students");
        }
    }
}
