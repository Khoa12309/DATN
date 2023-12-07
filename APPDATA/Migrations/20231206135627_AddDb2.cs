using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APPDATA.Migrations
{
    public partial class AddDb2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdVoucher",
                table: "Accounts");

            migrationBuilder.CreateTable(
                name: "VoucherForAccs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Id_Account = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id_Voucher = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherForAccs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherForAccs_Accounts_Id_Account",
                        column: x => x.Id_Account,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoucherForAccs_Vouchers_Id_Voucher",
                        column: x => x.Id_Voucher,
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VoucherForAccs_Id_Account",
                table: "VoucherForAccs",
                column: "Id_Account");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherForAccs_Id_Voucher",
                table: "VoucherForAccs",
                column: "Id_Voucher");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoucherForAccs");

            migrationBuilder.AddColumn<Guid>(
                name: "IdVoucher",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
