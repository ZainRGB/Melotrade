using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Melotrade.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessControl",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "fname",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sname",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessControl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "fname",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "sname",
                table: "AspNetUsers");
        }
    }
}
