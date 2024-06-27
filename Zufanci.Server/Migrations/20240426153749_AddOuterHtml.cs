using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zufanci.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddOuterHtml : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OuterHtml",
                table: "MonitoringItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OuterHtml",
                table: "MonitoringItems");
        }
    }
}
