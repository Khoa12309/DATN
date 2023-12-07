using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APPDATA.Migrations
{
    public partial class AddDb1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Vouchers_VoucherId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_VoucherId",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "VoucherId",
                table: "Accounts",
                newName: "IdVoucher");

            migrationBuilder.CreateTable(
                name: "AccountVoucher",
                columns: table => new
                {
                    AccountsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoucherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountVoucher", x => new { x.AccountsId, x.VoucherId });
                    table.ForeignKey(
                        name: "FK_AccountVoucher_Accounts_AccountsId",
                        column: x => x.AccountsId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountVoucher_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountVoucher_VoucherId",
                table: "AccountVoucher",
                column: "VoucherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountVoucher");

            migrationBuilder.RenameColumn(
                name: "IdVoucher",
                table: "Accounts",
                newName: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_VoucherId",
                table: "Accounts",
                column: "VoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Vouchers_VoucherId",
                table: "Accounts",
                column: "VoucherId",
                principalTable: "Vouchers",
                principalColumn: "Id");
        }
    }
}
