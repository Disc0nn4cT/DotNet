using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPagesYacht.Migrations
{
    /// <inheritdoc />
    public partial class Reviews_AdminReply_Add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AdminRepliedAt",
                table: "Reviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminReply",
                table: "Reviews",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminReplyBy",
                table: "Reviews",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminRepliedAt",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "AdminReply",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "AdminReplyBy",
                table: "Reviews");
        }
    }
}
