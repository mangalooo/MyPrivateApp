using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPrivateApp.Migrations
{
    /// <inheritdoc />
    public partial class newModelsFarmWorksPlanningAndCompleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FarmWorksPlanning",
                columns: table => new
                {
                    FarmWorksPlanningsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanningDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Place = table.Column<int>(type: "int", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prioritize = table.Column<int>(type: "int", nullable: false),
                    Todo = table.Column<int>(type: "int", nullable: false),
                    Hectare = table.Column<double>(type: "float", nullable: false),
                    Hours = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmWorksPlanning", x => x.FarmWorksPlanningsId);
                });

            migrationBuilder.CreateTable(
                name: "FarmWorksPlanningCompleted",
                columns: table => new
                {
                    FarmWorksPlanningCompletedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanningDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Place = table.Column<int>(type: "int", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prioritize = table.Column<int>(type: "int", nullable: false),
                    Todo = table.Column<int>(type: "int", nullable: false),
                    Hectare = table.Column<double>(type: "float", nullable: false),
                    Hours = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmWorksPlanningCompleted", x => x.FarmWorksPlanningCompletedId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FarmWorksPlanning");

            migrationBuilder.DropTable(
                name: "FarmWorksPlanningCompleted");
        }
    }
}
