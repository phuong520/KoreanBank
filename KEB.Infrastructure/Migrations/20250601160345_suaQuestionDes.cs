using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KEB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class suaQuestionDes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880000"),
                columns: new[] { "NotificationId", "UserName" },
                values: new object[] { new Guid("90a52dbd-bf4e-4950-aad7-be3ed7634c7d"), "phuonght40" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880001"),
                columns: new[] { "NotificationId", "UserName" },
                values: new object[] { new Guid("d0c29bb4-8362-4bbd-a431-8ed5a6f9ad75"), "phongnt1" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880002"),
                columns: new[] { "NotificationId", "UserName" },
                values: new object[] { new Guid("add58391-87a2-4882-9929-d8b967dfbf5d"), "xuanlt14" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Question",
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
                columns: new[] { "NotificationId", "UserName" },
                values: new object[] { new Guid("854e9a4a-1827-4753-a267-823e36a683f2"), "admin" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880001"),
                columns: new[] { "NotificationId", "UserName" },
                values: new object[] { new Guid("73f5a628-9cb1-4c50-8d04-419203bffc19"), "teamlead" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("a089198c-ed4c-4294-9e62-ac9a09880002"),
                columns: new[] { "NotificationId", "UserName" },
                values: new object[] { new Guid("6836861f-4071-4844-a5c6-98267b1ed8fb"), "lecturer" });
        }
    }
}
