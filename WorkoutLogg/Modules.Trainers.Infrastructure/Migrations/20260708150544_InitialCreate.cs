using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Trainers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "trainers");

            migrationBuilder.CreateTable(
                name: "TrainerProfiles",
                schema: "trainers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Specializations = table.Column<int>(type: "integer", nullable: false),
                    Experience = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Formats = table.Column<int>(type: "integer", nullable: false),
                    PricePerSession = table.Column<int>(type: "integer", nullable: false),
                    About = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerProfiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainerProfiles_IsActive",
                schema: "trainers",
                table: "TrainerProfiles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerProfiles_UserId",
                schema: "trainers",
                table: "TrainerProfiles",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainerProfiles",
                schema: "trainers");
        }
    }
}
