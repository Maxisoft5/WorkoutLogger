using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Trainers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasVerifiedBadge",
                schema: "trainers",
                table: "TrainerProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerificationBadge",
                schema: "trainers",
                table: "TrainerProfiles",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TrainerVerifications",
                schema: "trainers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainerUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ModeratorComment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Badge = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    SubmittedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerVerifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VerificationDocuments",
                schema: "trainers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VerificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    UploadedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationDocuments_TrainerVerifications_VerificationId",
                        column: x => x.VerificationId,
                        principalSchema: "trainers",
                        principalTable: "TrainerVerifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainerVerifications_TrainerUserId",
                schema: "trainers",
                table: "TrainerVerifications",
                column: "TrainerUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VerificationDocuments_VerificationId",
                schema: "trainers",
                table: "VerificationDocuments",
                column: "VerificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerificationDocuments",
                schema: "trainers");

            migrationBuilder.DropTable(
                name: "TrainerVerifications",
                schema: "trainers");

            migrationBuilder.DropColumn(
                name: "HasVerifiedBadge",
                schema: "trainers",
                table: "TrainerProfiles");

            migrationBuilder.DropColumn(
                name: "VerificationBadge",
                schema: "trainers",
                table: "TrainerProfiles");
        }
    }
}
