using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Trainers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrainingRequests",
                schema: "trainers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    TrainerUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    Goal = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Formats = table.Column<int>(type: "integer", nullable: false),
                    Schedule = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Budget = table.Column<int>(type: "integer", nullable: true),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DeclineReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RespondedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingRequests_StudentUserId_Status",
                schema: "trainers",
                table: "TrainingRequests",
                columns: new[] { "StudentUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingRequests_TrainerUserId_Status",
                schema: "trainers",
                table: "TrainingRequests",
                columns: new[] { "TrainerUserId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainingRequests",
                schema: "trainers");
        }
    }
}
