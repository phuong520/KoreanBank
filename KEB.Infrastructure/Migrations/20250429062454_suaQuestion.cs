using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KEB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class suaQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentFileName",
                table: "Question");

            migrationBuilder.AddColumn<Guid>(
                name: "AttachmentFileNameId",
                table: "Question",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Question_AttachmentFileNameId",
                table: "Question",
                column: "AttachmentFileNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_ImageFile_AttachmentFileNameId",
                table: "Question",
                column: "AttachmentFileNameId",
                principalTable: "ImageFile",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_ImageFile_AttachmentFileNameId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_AttachmentFileNameId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "AttachmentFileNameId",
                table: "Question");

            migrationBuilder.AddColumn<string>(
                name: "AttachmentFileName",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
