using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Trainers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvailabilitySlots",
                schema: "trainers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainerUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    StartUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsBooked = table.Column<bool>(type: "boolean", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailabilitySlots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                schema: "trainers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    TrainerUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    SlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    StudentNote = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CancelledBy = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    IsLateCancel = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConfirmedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilitySlots_TrainerUserId_StartUtc_IsBooked",
                schema: "trainers",
                table: "AvailabilitySlots",
                columns: new[] { "TrainerUserId", "StartUtc", "IsBooked" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SlotId",
                schema: "trainers",
                table: "Bookings",
                column: "SlotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StudentUserId_Status",
                schema: "trainers",
                table: "Bookings",
                columns: new[] { "StudentUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TrainerUserId_Status",
                schema: "trainers",
                table: "Bookings",
                columns: new[] { "TrainerUserId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings",
                schema: "trainers");

            migrationBuilder.DropTable(
                name: "AvailabilitySlots",
                schema: "trainers");
        }
    }
}
