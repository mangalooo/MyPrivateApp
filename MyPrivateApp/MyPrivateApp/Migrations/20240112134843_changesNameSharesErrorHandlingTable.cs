using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class changesNameSharesErrorHandlingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SharesErrorHandling",
                table: "SharesErrorHandling");

            migrationBuilder.RenameTable(
                name: "SharesErrorHandling",
                newName: "SharesErrorHandlings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SharesErrorHandlings",
                table: "SharesErrorHandlings",
                column: "ErrorHandlingsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SharesErrorHandlings",
                table: "SharesErrorHandlings");

            migrationBuilder.RenameTable(
                name: "SharesErrorHandlings",
                newName: "SharesErrorHandling");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SharesErrorHandling",
                table: "SharesErrorHandling",
                column: "ErrorHandlingsId");
        }
    }
}
