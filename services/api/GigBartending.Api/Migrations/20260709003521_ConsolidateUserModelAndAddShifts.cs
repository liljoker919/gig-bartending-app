using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GigBartending.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidateUserModelAndAddShifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_AspNetUsers_ApplicationUserId",
                table: "Shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_LegacyUsers_VenueId",
                table: "Shifts");

            migrationBuilder.DropTable(
                name: "LegacyUsers");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Shifts",
                newName: "AcceptedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_ApplicationUserId",
                table: "Shifts",
                newName: "IX_Shifts_AcceptedByUserId");

            migrationBuilder.AlterColumn<string>(
                name: "VenueId",
                table: "Shifts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ShiftRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftId = table.Column<int>(type: "int", nullable: false),
                    BartenderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftRequests_AspNetUsers_BartenderId",
                        column: x => x.BartenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftRequests_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftRequests_BartenderId",
                table: "ShiftRequests",
                column: "BartenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftRequests_ShiftId_BartenderId",
                table: "ShiftRequests",
                columns: new[] { "ShiftId", "BartenderId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_AspNetUsers_AcceptedByUserId",
                table: "Shifts",
                column: "AcceptedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_AspNetUsers_VenueId",
                table: "Shifts",
                column: "VenueId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_AspNetUsers_AcceptedByUserId",
                table: "Shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_AspNetUsers_VenueId",
                table: "Shifts");

            migrationBuilder.DropTable(
                name: "ShiftRequests");

            migrationBuilder.RenameColumn(
                name: "AcceptedByUserId",
                table: "Shifts",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_AcceptedByUserId",
                table: "Shifts",
                newName: "IX_Shifts_ApplicationUserId");

            migrationBuilder.AlterColumn<int>(
                name: "VenueId",
                table: "Shifts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "LegacyUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegacyUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LegacyUsers_Email",
                table: "LegacyUsers",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_AspNetUsers_ApplicationUserId",
                table: "Shifts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_LegacyUsers_VenueId",
                table: "Shifts",
                column: "VenueId",
                principalTable: "LegacyUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
