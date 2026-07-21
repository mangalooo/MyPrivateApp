using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class ConnectFarmWorksToFarmWorksPlanning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Accessories",
                table: "FarmWorks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FarmWorksPlanningsId",
                table: "FarmWorks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Todo",
                table: "FarmWorks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Tools",
                table: "FarmWorks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FarmWorks_FarmWorksPlanningsId",
                table: "FarmWorks",
                column: "FarmWorksPlanningsId");

            migrationBuilder.AddForeignKey(
                name: "FK_FarmWorks_FarmWorksPlanning_FarmWorksPlanningsId",
                table: "FarmWorks",
                column: "FarmWorksPlanningsId",
                principalTable: "FarmWorksPlanning",
                principalColumn: "FarmWorksPlanningsId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmWorks_FarmWorksPlanning_FarmWorksPlanningsId",
                table: "FarmWorks");

            migrationBuilder.DropIndex(
                name: "IX_FarmWorks_FarmWorksPlanningsId",
                table: "FarmWorks");

            migrationBuilder.DropColumn(
                name: "Accessories",
                table: "FarmWorks");

            migrationBuilder.DropColumn(
                name: "FarmWorksPlanningsId",
                table: "FarmWorks");

            migrationBuilder.DropColumn(
                name: "Todo",
                table: "FarmWorks");

            migrationBuilder.DropColumn(
                name: "Tools",
                table: "FarmWorks");
        }
    }
}
