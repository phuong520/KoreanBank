using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KEB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class suaExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exam_User_ReviewerId",
                table: "Exam");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Topic",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
                name: "IX_Exam_HostId",
                table: "Exam",
                column: "HostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exam_User_HostId",
                table: "Exam",
                column: "HostId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exam_User_ReviewerId",
                table: "Exam",
                column: "ReviewerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exam_User_HostId",
                table: "Exam");

            migrationBuilder.DropForeignKey(
                name: "FK_Exam_User_ReviewerId",
                table: "Exam");

            migrationBuilder.DropIndex(
                name: "IX_Exam_HostId",
                table: "Exam");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Topic",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Exam_User_ReviewerId",
                table: "Exam",
                column: "ReviewerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
