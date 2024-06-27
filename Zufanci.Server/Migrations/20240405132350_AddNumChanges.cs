using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zufanci.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddNumChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumChanges",
                table: "MonitoringItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumChanges",
                table: "MonitoringItems");
        }
    }
}
