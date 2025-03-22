using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AscomPayPG.Migrations
{
    public partial class Walllettypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "WalletTypeId",
                table: "UserWallets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TransactionRetries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsOffline = table.Column<bool>(type: "bit", nullable: false),
                    HasbeenSettled = table.Column<bool>(type: "bit", nullable: false),
                    ReceiverAccountOrWallet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderAccountOrWallet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverWalletId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderWalletId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hasPostedSuccessfully = table.Column<bool>(type: "bit", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    transactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverAccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Decription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfflineId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionRetries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeprecated = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserWallets_WalletTypeId",
                table: "UserWallets",
                column: "WalletTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWallets_WalletType_WalletTypeId",
                table: "UserWallets",
                column: "WalletTypeId",
                principalTable: "WalletType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWallets_WalletType_WalletTypeId",
                table: "UserWallets");

            migrationBuilder.DropTable(
                name: "TransactionRetries");

            migrationBuilder.DropTable(
                name: "WalletType");

            migrationBuilder.DropIndex(
                name: "IX_UserWallets_WalletTypeId",
                table: "UserWallets");

            migrationBuilder.AlterColumn<Guid>(
                name: "WalletTypeId",
                table: "UserWallets",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
