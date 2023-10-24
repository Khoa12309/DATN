using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APPDATA.Migrations
{
    public partial class lan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Suppliers_Id_supplier",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_Id_supplier",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Id_supplier",
                table: "Products");

            migrationBuilder.AddColumn<Guid>(
                name: "Id_supplier",
                table: "ProductDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Suppliers_Id_Product",
                table: "ProductDetails",
                column: "Id_Product",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Suppliers_Id_Product",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "Id_supplier",
                table: "ProductDetails");

            migrationBuilder.AddColumn<Guid>(
                name: "Id_supplier",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Id_supplier",
                table: "Products",
                column: "Id_supplier");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Suppliers_Id_supplier",
                table: "Products",
                column: "Id_supplier",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }
    }
}
