using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AscomPayPG.Migrations.AppLogDB
{
    public partial class maillog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationLog",
                columns: table => new
                {
                    NlogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Origin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    INotificationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SRecipient = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SCc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SSubject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IHasAttachment = table.Column<bool>(type: "bit", nullable: true),
                    SAttachmentCount = table.Column<int>(type: "int", nullable: true),
                    IStatus = table.Column<int>(type: "int", nullable: true),
                    ITryCount = table.Column<int>(type: "int", nullable: true),
                    DLastTriedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DCreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SComment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationLog", x => x.NlogId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationLog");
        }
    }
}
