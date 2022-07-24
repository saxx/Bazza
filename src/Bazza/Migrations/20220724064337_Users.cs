using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bazza.Migrations
{
    public partial class Users : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StringValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberValue = table.Column<int>(type: "int", nullable: true),
                    DateTimeValue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiresPasswordReset = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordSetUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
