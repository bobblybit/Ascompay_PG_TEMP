using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AscomPayPG.Migrations
{
    public partial class webHook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddColumn<bool>(
                name: "IsSettled",
                table: "Webhook",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSettled",
                table: "Webhook");
        }
    }
}
