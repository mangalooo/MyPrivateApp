using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class FrozenFoodsChangesDb1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FreezerId",
                table: "FrozenFoods",
                newName: "FrozenFoodsId");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "FrozenFoods",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "FrozenFoods");

            migrationBuilder.RenameColumn(
                name: "FrozenFoodsId",
                table: "FrozenFoods",
                newName: "FreezerId");
        }
    }
}
