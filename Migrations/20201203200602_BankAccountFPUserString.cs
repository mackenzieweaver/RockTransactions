using Microsoft.EntityFrameworkCore.Migrations;

namespace RockTransactions.Migrations
{
    public partial class BankAccountFPUserString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccount_AspNetUsers_FPUserId1",
                table: "BankAccount");

            migrationBuilder.DropIndex(
                name: "IX_BankAccount_FPUserId1",
                table: "BankAccount");

            migrationBuilder.DropColumn(
                name: "FPUserId1",
                table: "BankAccount");

            migrationBuilder.AlterColumn<string>(
                name: "FPUserId",
                table: "BankAccount",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_FPUserId",
                table: "BankAccount",
                column: "FPUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccount_AspNetUsers_FPUserId",
                table: "BankAccount",
                column: "FPUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccount_AspNetUsers_FPUserId",
                table: "BankAccount");

            migrationBuilder.DropIndex(
                name: "IX_BankAccount_FPUserId",
                table: "BankAccount");

            migrationBuilder.AlterColumn<int>(
                name: "FPUserId",
                table: "BankAccount",
                type: "integer",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FPUserId1",
                table: "BankAccount",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_FPUserId1",
                table: "BankAccount",
                column: "FPUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccount_AspNetUsers_FPUserId1",
                table: "BankAccount",
                column: "FPUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
