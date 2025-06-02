using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KEB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class suaPaperEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentUrl",
                table: "Paper");

            migrationBuilder.DropColumn(
                name: "PaperFileUrl",
                table: "Paper");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880000"),
                column: "NotificationId",
                value: new Guid("c022cff3-5688-4d90-abc2-960443f43e7f"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880001"),
                column: "NotificationId",
                value: new Guid("968a161a-d030-43e2-80de-7b8793966798"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880002"),
                column: "NotificationId",
                value: new Guid("1644d6b8-ef9b-4c54-92f2-bef886a2754d"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentUrl",
                table: "Paper",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaperFileUrl",
                table: "Paper",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880000"),
                column: "NotificationId",
                value: new Guid("90a52dbd-bf4e-4950-aad7-be3ed7634c7d"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880001"),
                column: "NotificationId",
                value: new Guid("d0c29bb4-8362-4bbd-a431-8ed5a6f9ad75"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880002"),
                column: "NotificationId",
                value: new Guid("add58391-87a2-4882-9929-d8b967dfbf5d"));
        }
    }
}
