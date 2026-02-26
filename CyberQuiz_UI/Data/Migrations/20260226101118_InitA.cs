using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CyberQuiz_UI.Migrations
{
    /// <inheritdoc />
    public partial class InitA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "001", 0, "7787a1c3-96c0-4c41-af34-6f27ab092c8a", "user@example.com", true, false, null, "USER@EXAMPLE.COM", "USER", "AQAAAAIAAYagAAAAEFHM75ycAE7U4myexV9bj22BEDaScpKLVvmpjpdNLeNeF5q79wrMPSJ3jlYx5kemGg==", null, false, "9cc2fdc4-66cb-410a-8694-6b3713c72c8e", false, "user" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "001");
        }
    }
}
