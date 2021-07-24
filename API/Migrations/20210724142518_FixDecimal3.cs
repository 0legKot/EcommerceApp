using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class FixDecimal3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(38,19)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,19)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "OrderProducts",
                type: "decimal(38,19)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,19)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(19,19)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,19)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "OrderProducts",
                type: "decimal(19,19)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,19)");
        }
    }
}
