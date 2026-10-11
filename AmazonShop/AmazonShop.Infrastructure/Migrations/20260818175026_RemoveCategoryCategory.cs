using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmazonShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCategoryCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Categories_CategoryId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_CategoryId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "CartItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CategoryId",
                table: "CartItems",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Categories_CategoryId",
                table: "CartItems",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
