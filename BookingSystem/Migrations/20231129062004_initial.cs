using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSystem.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    PackageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.PackageID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "ClassSchedules",
                columns: table => new
                {
                    ClassScheduleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequiredCredits = table.Column<int>(type: "int", nullable: false),
                    MaximumCapacity = table.Column<int>(type: "int", nullable: false),
                    RefundDone = table.Column<bool>(type: "bit", nullable: true),
                    PackageID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSchedules", x => x.ClassScheduleID);
                    table.ForeignKey(
                        name: "FK_ClassSchedules_Packages_PackageID",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID");
                });

            migrationBuilder.CreateTable(
                name: "UserPackages",
                columns: table => new
                {
                    UserPackageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    PackageID = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RemainingCredits = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPackages", x => x.UserPackageID);
                    table.ForeignKey(
                        name: "FK_UserPackages_Packages_PackageID",
                        column: x => x.PackageID,
                        principalTable: "Packages",
                        principalColumn: "PackageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPackages_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    ClassScheduleID = table.Column<int>(type: "int", nullable: false),
                    BookingTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingID);
                    table.ForeignKey(
                        name: "FK_Bookings_ClassSchedules_ClassScheduleID",
                        column: x => x.ClassScheduleID,
                        principalTable: "ClassSchedules",
                        principalColumn: "ClassScheduleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Waitlists",
                columns: table => new
                {
                    WaitlistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClassScheduleId = table.Column<int>(type: "int", nullable: false),
                    WaitlistTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waitlists", x => x.WaitlistId);
                    table.ForeignKey(
                        name: "FK_Waitlists_ClassSchedules_ClassScheduleId",
                        column: x => x.ClassScheduleId,
                        principalTable: "ClassSchedules",
                        principalColumn: "ClassScheduleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Waitlists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ClassSchedules",
                columns: new[] { "ClassScheduleID", "ClassName", "Country", "EndTime", "MaximumCapacity", "PackageID", "RefundDone", "RequiredCredits", "StartTime" },
                values: new object[,]
                {
                    { 1, "Yoga Class SG", "SG", new DateTime(2023, 11, 30, 16, 20, 4, 423, DateTimeKind.Utc).AddTicks(6857), 10, null, null, 1, new DateTime(2023, 11, 30, 15, 20, 4, 423, DateTimeKind.Utc).AddTicks(6852) },
                    { 2, "Zumba Class MY 1", "MM", new DateTime(2023, 11, 30, 16, 20, 4, 423, DateTimeKind.Utc).AddTicks(6861), 2, null, null, 2, new DateTime(2023, 11, 30, 15, 20, 4, 423, DateTimeKind.Utc).AddTicks(6860) },
                    { 3, "Zumba Class MY 2", "MY", new DateTime(2023, 11, 30, 13, 20, 4, 423, DateTimeKind.Utc).AddTicks(6863), 4, null, null, 2, new DateTime(2023, 11, 30, 12, 20, 4, 423, DateTimeKind.Utc).AddTicks(6863) },
                    { 4, "Zumba Class MY 3", "SG", new DateTime(2023, 11, 30, 11, 20, 4, 423, DateTimeKind.Utc).AddTicks(6866), 5, null, null, 3, new DateTime(2023, 11, 30, 11, 20, 4, 423, DateTimeKind.Utc).AddTicks(6865) },
                    { 5, "Zumba Class MY 4", "MM", new DateTime(2023, 11, 30, 10, 20, 4, 423, DateTimeKind.Utc).AddTicks(6868), 6, null, null, 1, new DateTime(2023, 11, 30, 10, 20, 4, 423, DateTimeKind.Utc).AddTicks(6867) },
                    { 6, "Zumba Class MY 5", "MY", new DateTime(2023, 11, 30, 9, 20, 4, 423, DateTimeKind.Utc).AddTicks(6870), 7, null, null, 2, new DateTime(2023, 11, 30, 8, 20, 4, 423, DateTimeKind.Utc).AddTicks(6869) }
                });

            migrationBuilder.InsertData(
                table: "Packages",
                columns: new[] { "PackageID", "Country", "Credits", "ExpiryDate", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "SG", 5, new DateTime(2024, 1, 29, 6, 20, 4, 423, DateTimeKind.Utc).AddTicks(6820), "Basic Package SG", 10.99m },
                    { 2, "MY", 10, new DateTime(2024, 2, 29, 6, 20, 4, 423, DateTimeKind.Utc).AddTicks(6831), "Premium Package MY", 29.99m },
                    { 3, "MM", 7, new DateTime(2024, 1, 29, 6, 20, 4, 423, DateTimeKind.Utc).AddTicks(6833), "Basic Package MM", 15.99m }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "Email", "IsEmailVerified", "Password", "Username" },
                values: new object[,]
                {
                    { 1, "user1@example.com", true, "hashed_password", "user1" },
                    { 2, "user2@example.com", true, "hashed_password", "user2" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ClassScheduleID",
                table: "Bookings",
                column: "ClassScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserID",
                table: "Bookings",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_PackageID",
                table: "ClassSchedules",
                column: "PackageID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPackages_PackageID",
                table: "UserPackages",
                column: "PackageID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPackages_UserID",
                table: "UserPackages",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Waitlists_ClassScheduleId",
                table: "Waitlists",
                column: "ClassScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Waitlists_UserId",
                table: "Waitlists",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "UserPackages");

            migrationBuilder.DropTable(
                name: "Waitlists");

            migrationBuilder.DropTable(
                name: "ClassSchedules");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Packages");
        }
    }
}
