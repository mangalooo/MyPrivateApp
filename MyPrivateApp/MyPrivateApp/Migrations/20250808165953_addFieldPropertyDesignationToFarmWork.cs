using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class addFieldPropertyDesignationToFarmWork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PropertyDesignation",
                table: "FarmWorksPlanningCompleted",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyDesignation",
                table: "FarmWorksPlanning",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyDesignation",
                table: "FarmWorks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyDesignation",
                table: "FarmWorksPlanningCompleted");

            migrationBuilder.DropColumn(
                name: "PropertyDesignation",
                table: "FarmWorksPlanning");

            migrationBuilder.DropColumn(
                name: "PropertyDesignation",
                table: "FarmWorks");
        }
    }
}
