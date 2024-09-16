using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class addTwoMoreFieldsToHuntingTowerInspection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "HuntingTowerInspections",
                newName: "LastInspected");

            migrationBuilder.AddColumn<bool>(
                name: "Inspected",
                table: "HuntingTowerInspections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InspectedTodo",
                table: "HuntingTowerInspections",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Inspected",
                table: "HuntingTowerInspections");

            migrationBuilder.DropColumn(
                name: "InspectedTodo",
                table: "HuntingTowerInspections");

            migrationBuilder.RenameColumn(
                name: "LastInspected",
                table: "HuntingTowerInspections",
                newName: "Date");
        }
    }
}
