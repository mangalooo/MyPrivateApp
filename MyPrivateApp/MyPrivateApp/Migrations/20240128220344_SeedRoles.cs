using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "04cb2980-5c81-483b-9782-96c57eb97d73", "ddfd43b6-7ef9-4cbc-8431-4dbf88ed1fc6", "Admin", "ADMIN" },
                    { "fd611758-d9e7-45b8-a494-620a1cdafd61", "dd80ca06-043b-4da5-9e22-fb2e0588a93e", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "04cb2980-5c81-483b-9782-96c57eb97d73");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fd611758-d9e7-45b8-a494-620a1cdafd61");
        }
    }
}
