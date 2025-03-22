using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AscomPayPG.Migrations
{
    public partial class accountjornalRemoverequestid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestTransactionId",
                table: "TransactionJournal");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestTransactionId",
                table: "TransactionJournal",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
