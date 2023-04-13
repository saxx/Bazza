using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bazza.Migrations
{
    /// <inheritdoc />
    public partial class Sales_and_UserRights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanManageAdmin",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanManagePersons",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanManageSales",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SaleId",
                table: "Articles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SaleUsername",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SaleUtc",
                table: "Articles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_SaleId",
                table: "Articles",
                column: "SaleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Sales_SaleId",
                table: "Articles",
                column: "SaleId",
                principalTable: "Sales",
                principalColumn: "Id");

            migrationBuilder.Sql("UPDATE [Users] SET CanManageAdmin=1, CanManageSales=1, CanManagePersons=1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Sales_SaleId",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Articles_SaleId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "CanManageAdmin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CanManagePersons",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CanManageSales",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SaleId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "SaleUsername",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "SaleUtc",
                table: "Articles");
        }
    }
}
