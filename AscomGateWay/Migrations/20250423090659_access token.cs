using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AscomPayPG.Migrations
{
    public partial class accesstoken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentGateWayMiddlewareLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PayloadHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PayloadSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    AccessTokenSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    AccessTokenHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    GenerationDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RquestUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGateWayMiddlewareLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentGateWayMiddlewareLogs");
        }
    }
}
