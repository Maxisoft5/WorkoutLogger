using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "repetition_number",
                schema: "users",
                table: "users_exercises");

            migrationBuilder.DropColumn(
                name: "rest_seconds",
                schema: "users",
                table: "users_exercises");

            migrationBuilder.DropColumn(
                name: "sets_number",
                schema: "users",
                table: "users_exercises");

            migrationBuilder.DropColumn(
                name: "weight_kg",
                schema: "users",
                table: "users_exercises");

            migrationBuilder.CreateTable(
                name: "exercise_sets",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    set_number = table.Column<int>(type: "integer", nullable: false),
                    reps = table.Column<int>(type: "integer", nullable: false),
                    weight_kg = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    rest_seconds = table.Column<int>(type: "integer", nullable: false, defaultValue: 60),
                    is_warmup = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now() at time zone 'utc'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_sets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_exercise_sets_users_exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalSchema: "users",
                        principalTable: "users_exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_exercise_sets_ExerciseId",
                schema: "users",
                table: "exercise_sets",
                column: "ExerciseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exercise_sets",
                schema: "users");

            migrationBuilder.AddColumn<int>(
                name: "repetition_number",
                schema: "users",
                table: "users_exercises",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "rest_seconds",
                schema: "users",
                table: "users_exercises",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sets_number",
                schema: "users",
                table: "users_exercises",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "weight_kg",
                schema: "users",
                table: "users_exercises",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
