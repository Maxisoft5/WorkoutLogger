using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserActiveRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveRole",
                schema: "users",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveRole",
                schema: "users",
                table: "users");
        }
    }
}
