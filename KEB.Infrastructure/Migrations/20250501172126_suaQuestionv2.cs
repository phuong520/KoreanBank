using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KEB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class suaQuestionv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_ImageFile_AttachmentFileNameId",
                table: "Question");

            migrationBuilder.RenameColumn(
                name: "AttachmentFileNameId",
                table: "Question",
                newName: "AttachmentFileId");

            migrationBuilder.RenameIndex(
                name: "IX_Question_AttachmentFileNameId",
                table: "Question",
                newName: "IX_Question_AttachmentFileId");

            migrationBuilder.AddColumn<Guid>(
                name: "AttachFileId",
                table: "Question",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880000"),
                columns: new[] { "Email", "NotificationId" },
                values: new object[] { "phuongminzy2@gmail.com", new Guid("bbf99366-5b8b-42ec-ba76-830bd7375866") });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880001"),
                columns: new[] { "Email", "NotificationId" },
                values: new object[] { "thanhphongh5201314@gmail.com", new Guid("d750be6d-bdfd-4b59-a112-b1f3862546ec") });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880002"),
                columns: new[] { "Email", "NotificationId" },
                values: new object[] { "blueberry5201314@gmail.com", new Guid("b19d10d2-7ef1-4eaa-a0a1-b4e0f5dfff28") });

            migrationBuilder.AddForeignKey(
                name: "FK_Question_ImageFile_AttachmentFileId",
                table: "Question",
                column: "AttachmentFileId",
                principalTable: "ImageFile",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_ImageFile_AttachmentFileId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "AttachFileId",
                table: "Question");

            migrationBuilder.RenameColumn(
                name: "AttachmentFileId",
                table: "Question",
                newName: "AttachmentFileNameId");

            migrationBuilder.RenameIndex(
                name: "IX_Question_AttachmentFileId",
                table: "Question",
                newName: "IX_Question_AttachmentFileNameId");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880000"),
                columns: new[] { "Email", "NotificationId" },
                values: new object[] { "phuonght40@fpt.com", new Guid("ec057c59-4a95-4b7e-9289-7c5ead55da65") });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880001"),
                columns: new[] { "Email", "NotificationId" },
                values: new object[] { "phongnt@gmail.com", new Guid("3b26b270-7834-4700-960a-5a52fe5a32e5") });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880002"),
                columns: new[] { "Email", "NotificationId" },
                values: new object[] { "thanhxuan123@gmail.com", new Guid("12012c24-47b3-499d-a37a-13f6c805536c") });

            migrationBuilder.AddForeignKey(
                name: "FK_Question_ImageFile_AttachmentFileNameId",
                table: "Question",
                column: "AttachmentFileNameId",
                principalTable: "ImageFile",
                principalColumn: "Id");
        }
    }
}
