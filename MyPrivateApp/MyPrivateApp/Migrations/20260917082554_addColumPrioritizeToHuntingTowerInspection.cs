using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class addColumPrioritizeToHuntingTowerInspection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Prioritize",
                table: "HuntingTowerInspections",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prioritize",
                table: "HuntingTowerInspections");
        }
    }
}
