using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KEB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class suaPaperDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachment",
                table: "PaperDetail");

            migrationBuilder.AddColumn<Guid>(
                name: "AttachmentId",
                table: "PaperDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880000"),
                column: "NotificationId",
                value: new Guid("e62f5589-7481-47ba-ad2d-f487bcb803dd"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880001"),
                column: "NotificationId",
                value: new Guid("6313bca6-ac41-4910-8cd6-3d130b672b23"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880002"),
                column: "NotificationId",
                value: new Guid("2b1a93ea-76cf-420f-afcd-85706b4cbac6"));

            migrationBuilder.CreateIndex(
                name: "IX_PaperDetail_AttachmentId",
                table: "PaperDetail",
                column: "AttachmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaperDetail_ImageFile_AttachmentId",
                table: "PaperDetail",
                column: "AttachmentId",
                principalTable: "ImageFile",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaperDetail_ImageFile_AttachmentId",
                table: "PaperDetail");

            migrationBuilder.DropIndex(
                name: "IX_PaperDetail_AttachmentId",
                table: "PaperDetail");

            migrationBuilder.DropColumn(
                name: "AttachmentId",
                table: "PaperDetail");

            migrationBuilder.AddColumn<string>(
                name: "Attachment",
                table: "PaperDetail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880000"),
                column: "NotificationId",
                value: new Guid("45c20f53-a38a-4e68-b31b-6224fc9d84bb"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880001"),
                column: "NotificationId",
                value: new Guid("cdc7c6f9-590b-469c-9363-a21ebf85caf4"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880002"),
                column: "NotificationId",
                value: new Guid("3338764b-89d3-4e26-8753-a029e368ac7d"));
        }
    }
}
