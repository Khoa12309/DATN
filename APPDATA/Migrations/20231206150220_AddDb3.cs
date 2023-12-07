using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APPDATA.Migrations
{
    public partial class AddDb3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "VoucherForAccs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "DiscountAmount",
                table: "VoucherForAccs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "VoucherForAccs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "VoucherForAccs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "VoucherForAccs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "VoucherForAccs");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "VoucherForAccs");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "VoucherForAccs");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "VoucherForAccs");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "VoucherForAccs");
        }
    }
}
