using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class addSharesSoldTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SharesSolds",
                columns: table => new
                {
                    SharesSoldId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    AmountSold = table.Column<double>(type: "float", nullable: false),
                    Brokerage = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfPurchase = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfSold = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HowMany = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeOfShares = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PricePerShares = table.Column<double>(type: "float", nullable: false),
                    PricePerSharesSold = table.Column<double>(type: "float", nullable: false),
                    MoneyProfitOrLoss = table.Column<double>(type: "float", nullable: false),
                    PercentProfitOrLoss = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ISIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharesSolds", x => x.SharesSoldId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharesSolds");
        }
    }
}
