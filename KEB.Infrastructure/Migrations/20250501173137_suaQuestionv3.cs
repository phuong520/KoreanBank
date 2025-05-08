using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KEB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class suaQuestionv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_ImageFile_AttachmentFileId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_AttachmentFileId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "AttachmentFileId",
                table: "Question");

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

            migrationBuilder.CreateIndex(
                name: "IX_Question_AttachFileId",
                table: "Question",
                column: "AttachFileId",
                unique: true,
                filter: "[AttachFileId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_ImageFile_AttachFileId",
                table: "Question",
                column: "AttachFileId",
                principalTable: "ImageFile",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_ImageFile_AttachFileId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_AttachFileId",
                table: "Question");

            migrationBuilder.AddColumn<Guid>(
                name: "AttachmentFileId",
                table: "Question",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880000"),
                column: "NotificationId",
                value: new Guid("bbf99366-5b8b-42ec-ba76-830bd7375866"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880001"),
                column: "NotificationId",
                value: new Guid("d750be6d-bdfd-4b59-a112-b1f3862546ec"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880002"),
                column: "NotificationId",
                value: new Guid("b19d10d2-7ef1-4eaa-a0a1-b4e0f5dfff28"));

            migrationBuilder.CreateIndex(
                name: "IX_Question_AttachmentFileId",
                table: "Question",
                column: "AttachmentFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_ImageFile_AttachmentFileId",
                table: "Question",
                column: "AttachmentFileId",
                principalTable: "ImageFile",
                principalColumn: "Id");
        }
    }
}
