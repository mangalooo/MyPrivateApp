using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class TripsChangesDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HowLongTravel",
                table: "Trips",
                newName: "HomeDate");

            migrationBuilder.AddColumn<double>(
                name: "HowManyDays",
                table: "Trips",
                type: "float",
                maxLength: 50,
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HowManyDays",
                table: "Trips");

            migrationBuilder.RenameColumn(
                name: "HomeDate",
                table: "Trips",
                newName: "HowLongTravel");
        }
    }
}
