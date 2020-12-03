using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RockTransactions.Migrations
{
    public partial class ModelsAndControllersWithViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HouseHoldId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HouseHold",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(maxLength: 40, nullable: false),
                    Greeting = table.Column<string>(maxLength: 40, nullable: true),
                    Established = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseHold", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HouseHoldId = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(maxLength: 40, nullable: true),
                    Description = table.Column<string>(maxLength: 40, nullable: true),
                    ContentType = table.Column<string>(maxLength: 40, nullable: true),
                    FileData = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachment_HouseHold_HouseHoldId",
                        column: x => x.HouseHoldId,
                        principalTable: "HouseHold",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HouseHoldId = table.Column<int>(nullable: false),
                    FPUserId = table.Column<int>(maxLength: 40, nullable: false),
                    FPUserId1 = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 40, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    StartingBalance = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(6,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccount_AspNetUsers_FPUserId1",
                        column: x => x.FPUserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankAccount_HouseHold_HouseHoldId",
                        column: x => x.HouseHoldId,
                        principalTable: "HouseHold",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HouseHoldId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 40, nullable: true),
                    Description = table.Column<string>(maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_HouseHold_HouseHoldId",
                        column: x => x.HouseHoldId,
                        principalTable: "HouseHold",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invitation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HouseHoldId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Expires = table.Column<DateTime>(nullable: false),
                    Accepted = table.Column<bool>(nullable: false),
                    EmailTo = table.Column<string>(maxLength: 40, nullable: true),
                    Subject = table.Column<string>(maxLength: 40, nullable: true),
                    Body = table.Column<string>(maxLength: 120, nullable: true),
                    Code = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitation_HouseHold_HouseHoldId",
                        column: x => x.HouseHoldId,
                        principalTable: "HouseHold",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HouseHoldId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Subject = table.Column<string>(maxLength: 40, nullable: true),
                    Body = table.Column<string>(maxLength: 40, nullable: true),
                    IsRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_HouseHold_HouseHoldId",
                        column: x => x.HouseHoldId,
                        principalTable: "HouseHold",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 40, nullable: true),
                    Description = table.Column<string>(maxLength: 40, nullable: true),
                    TargetAmount = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    ActualAmount = table.Column<decimal>(type: "decimal(6,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryItem_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryItemId = table.Column<int>(nullable: true),
                    BankAccountId = table.Column<int>(nullable: false),
                    FPUserId = table.Column<string>(maxLength: 40, nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Memo = table.Column<string>(maxLength: 40, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transaction_BankAccount_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transaction_CategoryItem_CategoryItemId",
                        column: x => x.CategoryItemId,
                        principalTable: "CategoryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaction_AspNetUsers_FPUserId",
                        column: x => x.FPUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_HouseHoldId",
                table: "AspNetUsers",
                column: "HouseHoldId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_HouseHoldId",
                table: "Attachment",
                column: "HouseHoldId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_FPUserId1",
                table: "BankAccount",
                column: "FPUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_HouseHoldId",
                table: "BankAccount",
                column: "HouseHoldId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_HouseHoldId",
                table: "Category",
                column: "HouseHoldId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryItem_CategoryId",
                table: "CategoryItem",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_HouseHoldId",
                table: "Invitation",
                column: "HouseHoldId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_HouseHoldId",
                table: "Notification",
                column: "HouseHoldId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_BankAccountId",
                table: "Transaction",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CategoryItemId",
                table: "Transaction",
                column: "CategoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_FPUserId",
                table: "Transaction",
                column: "FPUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_HouseHold_HouseHoldId",
                table: "AspNetUsers",
                column: "HouseHoldId",
                principalTable: "HouseHold",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_HouseHold_HouseHoldId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "Invitation");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "BankAccount");

            migrationBuilder.DropTable(
                name: "CategoryItem");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "HouseHold");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_HouseHoldId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HouseHoldId",
                table: "AspNetUsers");
        }
    }
}
