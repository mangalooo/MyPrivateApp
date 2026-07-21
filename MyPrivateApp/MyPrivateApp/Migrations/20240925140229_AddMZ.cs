using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMZ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManagerZonePurchasedPlayers",
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
                    table.PrimaryKey("PK_ManagerZonePurchasedPlayers", x => x.ManagerZonePurchasedPlayersId);
                });

            migrationBuilder.CreateTable(
                name: "ManagerZoneSoldPlayers",
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
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerZoneSoldPlayers", x => x.ManagerZoneSoldPlayerId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagerZonePurchasedPlayers");

            migrationBuilder.DropTable(
                name: "ManagerZoneSoldPlayers");
        }
    }
}
