using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeHundtingDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Place",
                table: "Huntings");

            migrationBuilder.AddColumn<int>(
                name: "HuntingPlaces",
                table: "Huntings",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HuntingPlaces",
                table: "Huntings");

            migrationBuilder.AddColumn<string>(
                name: "Place",
                table: "Huntings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
