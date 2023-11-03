using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APPDATA.Migrations
{
    public partial class db3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Categories_Id_Product",
                table: "ProductDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Colors_Id_Product",
                table: "ProductDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Materials_Id_Product",
                table: "ProductDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Sizes_Id_Product",
                table: "ProductDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Suppliers_Id_Product",
                table: "ProductDetails");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_Id_Category",
                table: "ProductDetails",
                column: "Id_Category");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_Id_Color",
                table: "ProductDetails",
                column: "Id_Color");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_Id_Material",
                table: "ProductDetails",
                column: "Id_Material");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_Id_Size",
                table: "ProductDetails",
                column: "Id_Size");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_Id_supplier",
                table: "ProductDetails",
                column: "Id_supplier");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Categories_Id_Category",
                table: "ProductDetails",
                column: "Id_Category",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Colors_Id_Color",
                table: "ProductDetails",
                column: "Id_Color",
                principalTable: "Colors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Materials_Id_Material",
                table: "ProductDetails",
                column: "Id_Material",
                principalTable: "Materials",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Sizes_Id_Size",
                table: "ProductDetails",
                column: "Id_Size",
                principalTable: "Sizes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Suppliers_Id_supplier",
                table: "ProductDetails",
                column: "Id_supplier",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Categories_Id_Category",
                table: "ProductDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Colors_Id_Color",
                table: "ProductDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Materials_Id_Material",
                table: "ProductDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Sizes_Id_Size",
                table: "ProductDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Suppliers_Id_supplier",
                table: "ProductDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProductDetails_Id_Category",
                table: "ProductDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProductDetails_Id_Color",
                table: "ProductDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProductDetails_Id_Material",
                table: "ProductDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProductDetails_Id_Size",
                table: "ProductDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProductDetails_Id_supplier",
                table: "ProductDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Categories_Id_Product",
                table: "ProductDetails",
                column: "Id_Product",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Colors_Id_Product",
                table: "ProductDetails",
                column: "Id_Product",
                principalTable: "Colors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Materials_Id_Product",
                table: "ProductDetails",
                column: "Id_Product",
                principalTable: "Materials",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Sizes_Id_Product",
                table: "ProductDetails",
                column: "Id_Product",
                principalTable: "Sizes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Suppliers_Id_Product",
                table: "ProductDetails",
                column: "Id_Product",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }
    }
}
