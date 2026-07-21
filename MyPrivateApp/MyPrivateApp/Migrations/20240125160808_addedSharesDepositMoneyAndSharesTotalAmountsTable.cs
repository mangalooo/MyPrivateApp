using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class addedSharesDepositMoneyAndSharesTotalAmountsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SharesDepositMoney",
                columns: table => new
                {
                    DepositMoneyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepositMoney = table.Column<double>(type: "float", nullable: false),
                    SubmitOrWithdraw = table.Column<int>(type: "int", nullable: false),
                    TypeOfTransaction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransferOptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharesDepositMoney", x => x.DepositMoneyId);
                });

            migrationBuilder.CreateTable(
                name: "SharesTotalAmounts",
                columns: table => new
                {
                    TotalAmountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalAmount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharesTotalAmounts", x => x.TotalAmountId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharesDepositMoney");

            migrationBuilder.DropTable(
                name: "SharesTotalAmounts");
        }
    }
}
