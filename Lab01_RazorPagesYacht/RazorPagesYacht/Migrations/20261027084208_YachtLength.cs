using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPagesYacht.Migrations
{
    /// <inheritdoc />
    public partial class YachtLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "Yacht",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Length",
                table: "Yacht");
        }
    }
}
