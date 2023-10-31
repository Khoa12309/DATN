using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APPDATA.Migrations
{
    public partial class AddDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_Carts_CartId",
                table: "CartDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_ProductDetails_CartId",
                table: "CartDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_ProductDetails_IdProductdetail",
                table: "Images");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdProductdetail",
                table: "Images",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CartId",
                table: "CartDetails",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_Carts_CartId",
                table: "CartDetails",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_ProductDetails_CartId",
                table: "CartDetails",
                column: "CartId",
                principalTable: "ProductDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_ProductDetails_IdProductdetail",
                table: "Images",
                column: "IdProductdetail",
                principalTable: "ProductDetails",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_Carts_CartId",
                table: "CartDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_ProductDetails_CartId",
                table: "CartDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_ProductDetails_IdProductdetail",
                table: "Images");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdProductdetail",
                table: "Images",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CartId",
                table: "CartDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_Carts_CartId",
                table: "CartDetails",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_ProductDetails_CartId",
                table: "CartDetails",
                column: "CartId",
                principalTable: "ProductDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_ProductDetails_IdProductdetail",
                table: "Images",
                column: "IdProductdetail",
                principalTable: "ProductDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
