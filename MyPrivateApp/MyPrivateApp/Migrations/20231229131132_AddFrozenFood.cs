using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFrozenFood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FrozenFoods",
                columns: table => new
                {
                    FrozenFoodsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Date = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Freezer = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    FreezerCompartment = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    WildMeat = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrozenFoods", x => x.FrozenFoodsId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FrozenFoods");
        }
    }
}
