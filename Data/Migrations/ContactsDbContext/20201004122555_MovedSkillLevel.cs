using Microsoft.EntityFrameworkCore.Migrations;

namespace ContactsApi.Data.Migrations.ContactsDbContext
{
    public partial class MovedSkillLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkillLevel",
                table: "Skills");

            migrationBuilder.AddColumn<int>(
                name: "SkillLevel",
                table: "ContactSkills",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkillLevel",
                table: "ContactSkills");

            migrationBuilder.AddColumn<int>(
                name: "SkillLevel",
                table: "Skills",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
