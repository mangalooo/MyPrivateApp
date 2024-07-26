using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class SharesFeeAddMoreErrorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Account",
                table: "SharesFees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyOrInformation",
                table: "SharesFees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateOfFee",
                table: "SharesFees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ISIN",
                table: "SharesFees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeOfTransaction",
                table: "SharesFees",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Account",
                table: "SharesFees");

            migrationBuilder.DropColumn(
                name: "CompanyOrInformation",
                table: "SharesFees");

            migrationBuilder.DropColumn(
                name: "DateOfFee",
                table: "SharesFees");

            migrationBuilder.DropColumn(
                name: "ISIN",
                table: "SharesFees");

            migrationBuilder.DropColumn(
                name: "TypeOfTransaction",
                table: "SharesFees");
        }
    }
}
