using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class ConnectFarmWorksToFarmWorksPlanningCompleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FarmWorksPlanningCompletedId",
                table: "FarmWorks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FarmWorks_FarmWorksPlanningCompletedId",
                table: "FarmWorks",
                column: "FarmWorksPlanningCompletedId");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmWorks_FarmWorksPlanningCompleted_FarmWorksPlanningCompletedId",
                table: "FarmWorks",
                column: "FarmWorksPlanningCompletedId",
                principalTable: "FarmWorksPlanningCompleted",
                principalColumn: "FarmWorksPlanningCompletedId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmWorks_FarmWorksPlanningCompleted_FarmWorksPlanningCompletedId",
                table: "FarmWorks");

            migrationBuilder.DropIndex(
                name: "IX_FarmWorks_FarmWorksPlanningCompletedId",
                table: "FarmWorks");

            migrationBuilder.DropColumn(
                name: "FarmWorksPlanningCompletedId",
                table: "FarmWorks");
        }
    }
}
