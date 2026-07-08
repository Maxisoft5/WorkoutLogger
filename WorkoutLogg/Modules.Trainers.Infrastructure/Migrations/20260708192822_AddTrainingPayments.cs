using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Trainers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrainingPayments",
                schema: "trainers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    TrainerUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    PriceFc = table.Column<int>(type: "integer", nullable: false),
                    CommissionFc = table.Column<int>(type: "integer", nullable: false),
                    PayoutFc = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolvedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPayments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPayments_StudentUserId_Status",
                schema: "trainers",
                table: "TrainingPayments",
                columns: new[] { "StudentUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPayments_TrainerUserId_Status",
                schema: "trainers",
                table: "TrainingPayments",
                columns: new[] { "TrainerUserId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainingPayments",
                schema: "trainers");
        }
    }
}
