using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class addTwoMoreFieldsToTowerInspectionMooseTowerAndWildBoarTower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MooseTower",
                table: "HuntingTowerInspections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WildBoarTower",
                table: "HuntingTowerInspections",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MooseTower",
                table: "HuntingTowerInspections");

            migrationBuilder.DropColumn(
                name: "WildBoarTower",
                table: "HuntingTowerInspections");
        }
    }
}
