using Microsoft.EntityFrameworkCore.Migrations;

namespace RockTransactions.Migrations
{
    public partial class BankAccountNullableFPUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FPUserId",
                table: "BankAccount",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 40);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FPUserId",
                table: "BankAccount",
                type: "integer",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(int),
                oldMaxLength: 40,
                oldNullable: true);
        }
    }
}
