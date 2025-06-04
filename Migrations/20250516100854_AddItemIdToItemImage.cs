using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Melotrade.Migrations
{
    /// <inheritdoc />
    public partial class AddItemIdToItemImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "ItemImages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ItemImages_ItemId",
                table: "ItemImages",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemImages_Items_ItemId",
                table: "ItemImages",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemImages_Items_ItemId",
                table: "ItemImages");

            migrationBuilder.DropIndex(
                name: "IX_ItemImages_ItemId",
                table: "ItemImages");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "ItemImages");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Items",
                type: "text",
                nullable: true);
        }
    }
}
