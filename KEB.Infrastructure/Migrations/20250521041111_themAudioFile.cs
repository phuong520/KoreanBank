using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KEB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class themAudioFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaperDetail_ImageFile_AttachmentId",
                table: "PaperDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_ImageFile_AttachFileId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_AttachFileId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_PaperDetail_AttachmentId",
                table: "PaperDetail");

            migrationBuilder.RenameColumn(
                name: "AttachFileId",
                table: "Question",
                newName: "AttachFileImageId");

            migrationBuilder.RenameColumn(
                name: "AttachmentId",
                table: "PaperDetail",
                newName: "AttachmentImageId");

            migrationBuilder.AddColumn<Guid>(
                name: "AttachFileAudioId",
                table: "Question",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AttachmentAudioId",
                table: "PaperDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880000"),
                column: "NotificationId",
                value: new Guid("854e9a4a-1827-4753-a267-823e36a683f2"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880001"),
                column: "NotificationId",
                value: new Guid("73f5a628-9cb1-4c50-8d04-419203bffc19"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880002"),
                column: "NotificationId",
                value: new Guid("6836861f-4071-4844-a5c6-98267b1ed8fb"));

            migrationBuilder.CreateIndex(
                name: "IX_Question_AttachFileAudioId",
                table: "Question",
                column: "AttachFileAudioId",
                unique: true,
                filter: "[AttachFileAudioId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Question_AttachFileImageId",
                table: "Question",
                column: "AttachFileImageId",
                unique: true,
                filter: "[AttachFileImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PaperDetail_AttachmentAudioId",
                table: "PaperDetail",
                column: "AttachmentAudioId",
                unique: true,
                filter: "[AttachmentAudioId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PaperDetail_AttachmentImageId",
                table: "PaperDetail",
                column: "AttachmentImageId",
                unique: true,
                filter: "[AttachmentImageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PaperDetail_ImageFile_AttachmentAudioId",
                table: "PaperDetail",
                column: "AttachmentAudioId",
                principalTable: "ImageFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaperDetail_ImageFile_AttachmentImageId",
                table: "PaperDetail",
                column: "AttachmentImageId",
                principalTable: "ImageFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_ImageFile_AttachFileAudioId",
                table: "Question",
                column: "AttachFileAudioId",
                principalTable: "ImageFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_ImageFile_AttachFileImageId",
                table: "Question",
                column: "AttachFileImageId",
                principalTable: "ImageFile",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaperDetail_ImageFile_AttachmentAudioId",
                table: "PaperDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_PaperDetail_ImageFile_AttachmentImageId",
                table: "PaperDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_ImageFile_AttachFileAudioId",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_ImageFile_AttachFileImageId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_AttachFileAudioId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_AttachFileImageId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_PaperDetail_AttachmentAudioId",
                table: "PaperDetail");

            migrationBuilder.DropIndex(
                name: "IX_PaperDetail_AttachmentImageId",
                table: "PaperDetail");

            migrationBuilder.DropColumn(
                name: "AttachFileAudioId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "AttachmentAudioId",
                table: "PaperDetail");

            migrationBuilder.RenameColumn(
                name: "AttachFileImageId",
                table: "Question",
                newName: "AttachFileId");

            migrationBuilder.RenameColumn(
                name: "AttachmentImageId",
                table: "PaperDetail",
                newName: "AttachmentId");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880000"),
                column: "NotificationId",
                value: new Guid("b1f7ff57-b5dd-46c3-9f4f-82e1d220bf0e"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880001"),
                column: "NotificationId",
                value: new Guid("98c9c161-01d3-4f3e-aa07-c85e0398184f"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880002"),
                column: "NotificationId",
                value: new Guid("906ae00a-ac10-431b-8141-7231b4ca9bd9"));

            migrationBuilder.CreateIndex(
                name: "IX_Question_AttachFileId",
                table: "Question",
                column: "AttachFileId",
                unique: true,
                filter: "[AttachFileId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PaperDetail_AttachmentId",
                table: "PaperDetail",
                column: "AttachmentId",
                unique: true,
                filter: "[AttachmentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PaperDetail_ImageFile_AttachmentId",
                table: "PaperDetail",
                column: "AttachmentId",
                principalTable: "ImageFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_ImageFile_AttachFileId",
                table: "Question",
                column: "AttachFileId",
                principalTable: "ImageFile",
                principalColumn: "Id");
        }
    }
}
