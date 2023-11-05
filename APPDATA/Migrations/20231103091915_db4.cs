using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APPDATA.Migrations
{
    public partial class db4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_ProductDetails_CartId",
                table: "CartDetails");

            migrationBuilder.RenameColumn(
                name: "ProductDetail",
                table: "CartDetails",
                newName: "ProductDetail_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_ProductDetail_ID",
                table: "CartDetails",
                column: "ProductDetail_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_ProductDetails_ProductDetail_ID",
                table: "CartDetails",
                column: "ProductDetail_ID",
                principalTable: "ProductDetails",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_ProductDetails_ProductDetail_ID",
                table: "CartDetails");

            migrationBuilder.DropIndex(
                name: "IX_CartDetails_ProductDetail_ID",
                table: "CartDetails");

            migrationBuilder.RenameColumn(
                name: "ProductDetail_ID",
                table: "CartDetails",
                newName: "ProductDetail");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_ProductDetails_CartId",
                table: "CartDetails",
                column: "CartId",
                principalTable: "ProductDetails",
                principalColumn: "Id");
        }
    }
}
