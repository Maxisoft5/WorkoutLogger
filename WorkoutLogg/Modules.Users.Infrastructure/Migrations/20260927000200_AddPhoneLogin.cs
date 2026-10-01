using Microsoft.EntityFrameworkCore.Migrations;

namespace Modules.Users.Infrastructure.Migrations;

public partial class AddPhoneLogin : Migration
{
    protected override void Up(MigrationBuilder b)
    {
        b.AddColumn<string>(name: "NormalizedPhoneNumber", schema: "users", table: "users", type: "character varying(16)", maxLength: 16, nullable: true);
        b.CreateIndex(name: "IX_users_NormalizedPhoneNumber", schema: "users", table: "users", column: "NormalizedPhoneNumber", unique: true);
    }
    protected override void Down(MigrationBuilder b)
    {
        b.DropIndex(name: "IX_users_NormalizedPhoneNumber", schema: "users", table: "users");
        b.DropColumn(name: "NormalizedPhoneNumber", schema: "users", table: "users");
    }
}
