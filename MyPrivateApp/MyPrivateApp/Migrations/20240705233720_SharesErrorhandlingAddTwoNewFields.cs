using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class SharesErrorhandlingAddTwoNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyOrInformation",
                table: "SharesErrorHandlings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeOfTransaction",
                table: "SharesErrorHandlings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyOrInformation",
                table: "SharesErrorHandlings");

            migrationBuilder.DropColumn(
                name: "TypeOfTransaction",
                table: "SharesErrorHandlings");
        }
    }
}
