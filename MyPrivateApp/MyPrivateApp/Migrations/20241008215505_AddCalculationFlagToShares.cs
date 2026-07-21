using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCalculationFlagToShares : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CalculationFlag",
                table: "SharesSolds",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CalculationFlag",
                table: "SharesSoldFunds",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CalculationFlag",
                table: "SharesInterestRates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CalculationFlag",
                table: "SharesFees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CalculationFlag",
                table: "SharesDividends",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculationFlag",
                table: "SharesSolds");

            migrationBuilder.DropColumn(
                name: "CalculationFlag",
                table: "SharesSoldFunds");

            migrationBuilder.DropColumn(
                name: "CalculationFlag",
                table: "SharesInterestRates");

            migrationBuilder.DropColumn(
                name: "CalculationFlag",
                table: "SharesFees");

            migrationBuilder.DropColumn(
                name: "CalculationFlag",
                table: "SharesDividends");
        }
    }
}
