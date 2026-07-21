using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFarming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FarmingsActive",
                columns: table => new
                {
                    FarmingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Place = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PutSeedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SetDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TakeUpDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HowMany = table.Column<int>(type: "int", nullable: false),
                    HowManySave = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmingsActive", x => x.FarmingId);
                });

            migrationBuilder.CreateTable(
                name: "FarmingsInactive",
                columns: table => new
                {
                    FarmingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InactiveDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Place = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PutSeedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SetDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TakeUpDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HowMany = table.Column<int>(type: "int", nullable: false),
                    HowManySave = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmingsInactive", x => x.FarmingId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FarmingsActive");

            migrationBuilder.DropTable(
                name: "FarmingsInactive");
        }
    }
}
