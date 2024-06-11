using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class AddHuntingMyLIstAndChangeNameONEnumsToFrezzer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Huntings");

            migrationBuilder.RenameColumn(
                name: "WildMeat",
                table: "FrozenFoods",
                newName: "Place");

            migrationBuilder.RenameColumn(
                name: "Freezer",
                table: "FrozenFoods",
                newName: "FrozenGoods");

            migrationBuilder.CreateTable(
                name: "HuntingMyList",
                columns: table => new
                {
                    HuntingMyListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WildAnimal = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Dog = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HuntingPlaces = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HuntingMyList", x => x.HuntingMyListId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HuntingMyList");

            migrationBuilder.RenameColumn(
                name: "Place",
                table: "FrozenFoods",
                newName: "WildMeat");

            migrationBuilder.RenameColumn(
                name: "FrozenGoods",
                table: "FrozenFoods",
                newName: "Freezer");

            migrationBuilder.CreateTable(
                name: "Huntings",
                columns: table => new
                {
                    HuntingsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Dog = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HuntingPlaces = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WildAnimal = table.Column<int>(type: "int", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Huntings", x => x.HuntingsId);
                });
        }
    }
}
