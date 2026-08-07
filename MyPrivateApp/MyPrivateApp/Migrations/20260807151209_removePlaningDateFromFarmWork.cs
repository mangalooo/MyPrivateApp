using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class removePlaningDateFromFarmWork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlanningDate",
                table: "FarmWorksPlanningCompleted");

            migrationBuilder.DropColumn(
                name: "PlanningDate",
                table: "FarmWorksPlanning");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlanningDate",
                table: "FarmWorksPlanningCompleted",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlanningDate",
                table: "FarmWorksPlanning",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
