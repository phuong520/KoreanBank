using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KEB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class suaPaperDetail2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaperDetail_AttachmentId",
                table: "PaperDetail");

            migrationBuilder.AlterColumn<Guid>(
                name: "AttachmentId",
                table: "PaperDetail",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880000"),
                column: "NotificationId",
                value: new Guid("d511b5a2-02b3-4e6e-8d1c-583cafc8195e"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880001"),
                column: "NotificationId",
                value: new Guid("901a909c-9895-42d3-b316-5795ed8ccc72"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880002"),
                column: "NotificationId",
                value: new Guid("664dee79-a16b-4553-b359-d79371fd9891"));

            migrationBuilder.CreateIndex(
                name: "IX_PaperDetail_AttachmentId",
                table: "PaperDetail",
                column: "AttachmentId",
                unique: true,
                filter: "[AttachmentId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaperDetail_AttachmentId",
                table: "PaperDetail");

            migrationBuilder.AlterColumn<Guid>(
                name: "AttachmentId",
                table: "PaperDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

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
        }
    }
}
