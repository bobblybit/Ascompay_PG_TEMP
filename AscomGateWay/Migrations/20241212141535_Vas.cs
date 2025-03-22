using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AscomPayPG.Migrations
{
    public partial class Vas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.CreateTable(
                name: "TransactionJournal",
                columns: table => new
                {
                    TransactionReference = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserUID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountOrWalletId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JournalType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentProvider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestTransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T_Charge = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    T_Vat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    T_Provider_Charges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    T_Marchant_Charges = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankCode = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeprecated = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionJournal", x => x.TransactionReference);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "TransactionJournal");

        }
    }
}
