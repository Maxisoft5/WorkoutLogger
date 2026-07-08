using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameExerciseComplexityColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "exercies_complexity",
                schema: "users",
                table: "users_exercises",
                newName: "exercise_complexity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "exercise_complexity",
                schema: "users",
                table: "users_exercises",
                newName: "exercies_complexity");
        }
    }
}
