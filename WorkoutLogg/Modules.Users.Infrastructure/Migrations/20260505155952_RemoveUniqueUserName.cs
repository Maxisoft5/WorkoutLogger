using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                schema: "users",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "users",
                table: "users",
                column: "NormalizedUserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                schema: "users",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "users",
                table: "users",
                column: "NormalizedUserName",
                unique: true);
        }
    }
}
