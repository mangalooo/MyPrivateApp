using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToFarmWorkPlaningAndCompletedFourHoursFieldsAndDeletedTodoField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Todo",
                table: "FarmWorksPlanningCompleted");

            migrationBuilder.DropColumn(
                name: "Todo",
                table: "FarmWorksPlanning");

            migrationBuilder.AddColumn<double>(
                name: "ClearingHours",
                table: "FarmWorksPlanningCompleted",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ForClearingHours",
                table: "FarmWorksPlanningCompleted",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ThinHours",
                table: "FarmWorksPlanningCompleted",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ToForwardHours",
                table: "FarmWorksPlanningCompleted",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ClearingHours",
                table: "FarmWorksPlanning",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ForClearingHours",
                table: "FarmWorksPlanning",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ThinHours",
                table: "FarmWorksPlanning",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ToForwardHours",
                table: "FarmWorksPlanning",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClearingHours",
                table: "FarmWorksPlanningCompleted");

            migrationBuilder.DropColumn(
                name: "ForClearingHours",
                table: "FarmWorksPlanningCompleted");

            migrationBuilder.DropColumn(
                name: "ThinHours",
                table: "FarmWorksPlanningCompleted");

            migrationBuilder.DropColumn(
                name: "ToForwardHours",
                table: "FarmWorksPlanningCompleted");

            migrationBuilder.DropColumn(
                name: "ClearingHours",
                table: "FarmWorksPlanning");

            migrationBuilder.DropColumn(
                name: "ForClearingHours",
                table: "FarmWorksPlanning");

            migrationBuilder.DropColumn(
                name: "ThinHours",
                table: "FarmWorksPlanning");

            migrationBuilder.DropColumn(
                name: "ToForwardHours",
                table: "FarmWorksPlanning");

            migrationBuilder.AddColumn<int>(
                name: "Todo",
                table: "FarmWorksPlanningCompleted",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Todo",
                table: "FarmWorksPlanning",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
