using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class ManagerZoneAddThreeMoreFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DaysInTheClub",
                table: "ManagerZonePurchasedPlayers",
                newName: "YearsOld");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ManagerZoneSoldPlayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "ManagerZoneSoldPlayers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "YearsOld",
                table: "ManagerZoneSoldPlayers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ManagerZonePurchasedPlayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "ManagerZonePurchasedPlayers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ManagerZoneSoldPlayers");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "ManagerZoneSoldPlayers");

            migrationBuilder.DropColumn(
                name: "YearsOld",
                table: "ManagerZoneSoldPlayers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ManagerZonePurchasedPlayers");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "ManagerZonePurchasedPlayers");

            migrationBuilder.RenameColumn(
                name: "YearsOld",
                table: "ManagerZonePurchasedPlayers",
                newName: "DaysInTheClub");
        }
    }
}
