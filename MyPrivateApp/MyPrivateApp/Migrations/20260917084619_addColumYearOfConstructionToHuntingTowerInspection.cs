using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class addColumYearOfConstructionToHuntingTowerInspection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YearOfConstruction",
                table: "HuntingTowerInspections",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearOfConstruction",
                table: "HuntingTowerInspections");
        }
    }
}
