using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class AddOneMoreFieldsAndChngesMZTablesNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagerZonePurchasedPlayers");

            migrationBuilder.DropTable(
                name: "ManagerZoneSoldPlayers");

            migrationBuilder.CreateTable(
                name: "MZPurchasedPlayers",
                columns: table => new
                {
                    ManagerZonePurchasedPlayersId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchasedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseAmount = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YearsOld = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Salary = table.Column<int>(type: "int", nullable: false),
                    SalarySaved = table.Column<int>(type: "int", nullable: false),
                    TrainingModeTotalCost = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MZPurchasedPlayers", x => x.ManagerZonePurchasedPlayersId);
                });

            migrationBuilder.CreateTable(
                name: "MZSoldPlayers",
                columns: table => new
                {
                    ManagerZoneSoldPlayerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchasedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoldDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseAmount = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YearsOld = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    SalaryTotal = table.Column<int>(type: "int", nullable: false),
                    DaysInTheClub = table.Column<int>(type: "int", nullable: false),
                    TrainingModeTotalCost = table.Column<int>(type: "int", nullable: false),
                    SoldAmount = table.Column<int>(type: "int", nullable: false),
                    MoneyProfitOrLoss = table.Column<int>(type: "int", nullable: false),
                    PercentProfitOrLoss = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MZSoldPlayers", x => x.ManagerZoneSoldPlayerId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MZPurchasedPlayers");

            migrationBuilder.DropTable(
                name: "MZSoldPlayers");

            migrationBuilder.CreateTable(
                name: "ManagerZonePurchasedPlayers",
                columns: table => new
                {
                    ManagerZonePurchasedPlayersId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Number = table.Column<int>(type: "int", nullable: false),
                    PurchaseAmount = table.Column<int>(type: "int", nullable: false),
                    PurchasedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salary = table.Column<int>(type: "int", nullable: false),
                    SalarySaved = table.Column<int>(type: "int", nullable: false),
                    TrainingModeTotalCost = table.Column<int>(type: "int", nullable: false),
                    YearsOld = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerZonePurchasedPlayers", x => x.ManagerZonePurchasedPlayersId);
                });

            migrationBuilder.CreateTable(
                name: "ManagerZoneSoldPlayers",
                columns: table => new
                {
                    ManagerZoneSoldPlayerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DaysInTheClub = table.Column<int>(type: "int", nullable: false),
                    MoneyProfitOrLoss = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Number = table.Column<int>(type: "int", nullable: false),
                    PurchaseAmount = table.Column<int>(type: "int", nullable: false),
                    PurchasedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalaryTotal = table.Column<int>(type: "int", nullable: false),
                    SoldAmount = table.Column<int>(type: "int", nullable: false),
                    SoldDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrainingModeTotalCost = table.Column<int>(type: "int", nullable: false),
                    YearsOld = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerZoneSoldPlayers", x => x.ManagerZoneSoldPlayerId);
                });
        }
    }
}
