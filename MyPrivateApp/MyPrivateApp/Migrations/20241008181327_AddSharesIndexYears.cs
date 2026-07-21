using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSharesIndexYears : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SharesProfitOrLossYears",
                columns: table => new
                {
                    SharesProfitOrLossYearsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SharesYear = table.Column<double>(type: "float", nullable: false),
                    FundsYear = table.Column<double>(type: "float", nullable: false),
                    DividendYear = table.Column<double>(type: "float", nullable: false),
                    InterestRatesYear = table.Column<double>(type: "float", nullable: false),
                    FeeYear = table.Column<double>(type: "float", nullable: false),
                    BrokerageYear = table.Column<double>(type: "float", nullable: false),
                    MoneyProfitOrLossYear = table.Column<double>(type: "float", nullable: false),
                    PercentProfitOrLossYear = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharesProfitOrLossYears", x => x.SharesProfitOrLossYearsId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharesProfitOrLossYears");
        }
    }
}
