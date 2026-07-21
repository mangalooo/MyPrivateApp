using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class addedSharesImportsFileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "04cb2980-5c81-483b-9782-96c57eb97d73");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fd611758-d9e7-45b8-a494-620a1cdafd61");

            migrationBuilder.CreateTable(
                name: "SharesImportsFiles",
                columns: table => new
                {
                    SharesImportsFileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumbersOfTransaction = table.Column<int>(type: "int", nullable: false),
                    Errors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharesImportsFiles", x => x.SharesImportsFileId);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "44e5e3c9-a26f-48e3-a82b-59614189179b", "53a719fa-a220-45ce-93c5-8e82c0a71471", "User", "USER" },
                    { "9792f270-8782-41ff-925b-2bbdbf27daeb", "96149f12-91d4-43cf-99fc-d85b18b8d17a", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharesImportsFiles");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "44e5e3c9-a26f-48e3-a82b-59614189179b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9792f270-8782-41ff-925b-2bbdbf27daeb");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "04cb2980-5c81-483b-9782-96c57eb97d73", "ddfd43b6-7ef9-4cbc-8431-4dbf88ed1fc6", "Admin", "ADMIN" },
                    { "fd611758-d9e7-45b8-a494-620a1cdafd61", "dd80ca06-043b-4da5-9e22-fb2e0588a93e", "User", "USER" }
                });
        }
    }
}
