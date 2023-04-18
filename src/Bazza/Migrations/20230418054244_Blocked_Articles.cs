using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bazza.Migrations
{
    /// <inheritdoc />
    public partial class Blocked_Articles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockedUsername",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BlockedUtc",
                table: "Articles",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockedUsername",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "BlockedUtc",
                table: "Articles");
        }
    }
}
