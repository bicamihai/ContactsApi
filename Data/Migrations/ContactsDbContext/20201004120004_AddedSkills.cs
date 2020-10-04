using Microsoft.EntityFrameworkCore.Migrations;

namespace ContactsApi.Data.Migrations.ContactsDbContext
{
    public partial class AddedSkills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactSkill_Contacts_ContactId",
                table: "ContactSkill");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactSkill_Skills_SkillId",
                table: "ContactSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactSkill",
                table: "ContactSkill");

            migrationBuilder.RenameTable(
                name: "ContactSkill",
                newName: "ContactSkills");

            migrationBuilder.RenameIndex(
                name: "IX_ContactSkill_SkillId",
                table: "ContactSkills",
                newName: "IX_ContactSkills_SkillId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactSkills",
                table: "ContactSkills",
                columns: new[] { "ContactId", "SkillId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ContactSkills_Contacts_ContactId",
                table: "ContactSkills",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactSkills_Skills_SkillId",
                table: "ContactSkills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactSkills_Contacts_ContactId",
                table: "ContactSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactSkills_Skills_SkillId",
                table: "ContactSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactSkills",
                table: "ContactSkills");

            migrationBuilder.RenameTable(
                name: "ContactSkills",
                newName: "ContactSkill");

            migrationBuilder.RenameIndex(
                name: "IX_ContactSkills_SkillId",
                table: "ContactSkill",
                newName: "IX_ContactSkill_SkillId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactSkill",
                table: "ContactSkill",
                columns: new[] { "ContactId", "SkillId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ContactSkill_Contacts_ContactId",
                table: "ContactSkill",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactSkill_Skills_SkillId",
                table: "ContactSkill",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
