using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APPDATA.Migrations
{
    public partial class AddDb1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppliesToOrders",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "AppliesToOrders",
                table: "VoucherForAccs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AppliesToOrders",
                table: "Vouchers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AppliesToOrders",
                table: "VoucherForAccs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
