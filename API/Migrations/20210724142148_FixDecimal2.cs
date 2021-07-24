using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class FixDecimal2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(19,19)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,18)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "OrderProducts",
                type: "decimal(19,19)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,18)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,19)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "OrderProducts",
                type: "decimal(18,18)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,19)");
        }
    }
}
