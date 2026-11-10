using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPagesYacht.Migrations
{
    /// <inheritdoc />
    public partial class YachtPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Yacht",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Yacht");
        }
    }
}
