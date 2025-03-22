using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AscomPayPG.Migrations
{
    public partial class accountjornal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionJournal",
                table: "TransactionJournal");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TransactionJournal",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionReference",
                table: "TransactionJournal",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionJournal",
                table: "TransactionJournal");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionReference",
                table: "TransactionJournal",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TransactionJournal",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionJournal",
                table: "TransactionJournal",
                column: "TransactionReference");
        }
    }
}
