using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KEB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class unit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageFile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Level",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LevelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Skill = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "References",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferenceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceAuthor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedYear = table.Column<int>(type: "int", nullable: false),
                    ReferencesLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_References", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Topic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TopicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topic", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExamTypeConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTypeConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamTypeConfiguration_Level_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Level",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<bool>(type: "bit", nullable: false),
                    Avatar = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_ImageFile_Avatar",
                        column: x => x.Avatar,
                        principalTable: "ImageFile",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LevelDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LevelDetail_Level_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Level",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LevelDetail_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamTypeConstraint",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Skill = table.Column<int>(type: "int", nullable: false),
                    TotalNumberOfQuestions = table.Column<int>(type: "int", nullable: false),
                    DurationInMinutes = table.Column<int>(type: "int", nullable: false),
                    NumberOfPaper = table.Column<int>(type: "int", nullable: false),
                    ExamTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTypeConstraint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamTypeConstraint_ExamTypeConfiguration_ExamTypeId",
                        column: x => x.ExamTypeId,
                        principalTable: "ExamTypeConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exam",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TakePlaceTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSuspended = table.Column<bool>(type: "bit", nullable: false),
                    ExamTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exam_ExamTypeConfiguration_ExamTypeId",
                        column: x => x.ExamTypeId,
                        principalTable: "ExamTypeConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exam_User_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemAccessLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetObject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAccessLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemAccessLog_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddQuestionTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ForMultiChoice = table.Column<bool>(type: "bit", nullable: false),
                    Difficult = table.Column<int>(type: "int", nullable: false),
                    NumberOfQuestion = table.Column<int>(type: "int", nullable: false),
                    ScheduleTaskId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeadLine = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    QuestionTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LevelDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddQuestionTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddQuestionTask_LevelDetail_LevelDetailId",
                        column: x => x.LevelDetailId,
                        principalTable: "LevelDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddQuestionTask_QuestionType_QuestionTypeId",
                        column: x => x.QuestionTypeId,
                        principalTable: "QuestionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddQuestionTask_User_AssignId",
                        column: x => x.AssignId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConstraintDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberOfQuestion = table.Column<int>(type: "int", nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    MarkPerQuestion = table.Column<float>(type: "real", nullable: false),
                    IsMultipleChoice = table.Column<bool>(type: "bit", nullable: false),
                    TopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamTypeConstraintId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConstraintDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConstraintDetail_ExamTypeConstraint_ExamTypeConstraintId",
                        column: x => x.ExamTypeConstraintId,
                        principalTable: "ExamTypeConstraint",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConstraintDetail_QuestionType_QuestionTypeId",
                        column: x => x.QuestionTypeId,
                        principalTable: "QuestionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConstraintDetail_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Paper",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaperName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Skill = table.Column<int>(type: "int", nullable: false),
                    PaperStatus = table.Column<int>(type: "int", nullable: false),
                    IsReviewed = table.Column<bool>(type: "bit", nullable: false),
                    PaperFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Paper_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    QuestionContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttachmentFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentDuration = table.Column<int>(type: "int", nullable: true),
                    IsMultipleChoice = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LevelDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_AddQuestionTask_TaskId",
                        column: x => x.TaskId,
                        principalTable: "AddQuestionTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Question_LevelDetail_LevelDetailId",
                        column: x => x.LevelDetailId,
                        principalTable: "LevelDetail",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Question_QuestionType_QuestionTypeId",
                        column: x => x.QuestionTypeId,
                        principalTable: "QuestionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Question_References_ReferenceId",
                        column: x => x.ReferenceId,
                        principalTable: "References",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Question_SystemAccessLog_LogId",
                        column: x => x.LogId,
                        principalTable: "SystemAccessLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnswerContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTrue = table.Column<bool>(type: "bit", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answer_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaperDetail",
                columns: table => new
                {
                    PaperId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Mark = table.Column<float>(type: "real", nullable: false),
                    OrderInPaper = table.Column<int>(type: "int", nullable: false),
                    Attachment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaperDetail", x => new { x.PaperId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_PaperDetail_Paper_PaperId",
                        column: x => x.PaperId,
                        principalTable: "Paper",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaperDetail_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Level",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsDeleted", "LevelName", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0a14f049-6666-4c9f-a834-d99b16176001"), new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Sơ cấp 1", new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-6666-4c9f-a834-d99b16176002"), new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Sơ cấp 2", new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-6666-4c9f-a834-d99b16176003"), new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Trung cấp 3", new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "QuestionType",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsDeleted", "Skill", "TypeContent", "TypeName", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0a14f049-4da1-4c9f-a834-d99b16176f50"), new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"), new DateTime(2025, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, "다음을 듣고 질문을 답하십시오", "Nghe và trả lời câu hỏi", new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"), new DateTime(2025, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-4da1-4c9f-a834-d99b16176f52"), new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"), new DateTime(2024, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 2, "다음을 듣고 알맟은 것을 고르십시오", "Tự sự về bản thân", new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"), new DateTime(2024, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-4da1-4c9f-a834-d99b16176f53"), new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"), new DateTime(2025, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 3, "다음을 듣고 알맟은 것을 고르십시오", "Chọn ngữ pháp đúng điền vào chỗ trống", new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"), new DateTime(2025, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-4da1-4c9f-a834-d99b16176f54"), new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"), new DateTime(2025, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 4, "다음을 듣고 알맟은 것을 고르십시오", "Viết câu 51,52", new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"), new DateTime(2025, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "References",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "IsDeleted", "PublishedYear", "ReferenceAuthor", "ReferenceName", "ReferencesLink", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0a14f049-4444-4c9f-a834-d99b16176000"), new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "No info", false, 2002, "No info", "Tài liệu khác", "No info", new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-4444-4c9f-a834-d99b16176001"), new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "No info", false, 2002, "No info", "Tiếng Hàn tổng hợp quyển 1", "No info", new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-4444-4c9f-a834-d99b16176002"), new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "No info", false, 2002, "No info", "Tiếng Hàn tổng hợp quyển 2", "No info", new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsDeleted", "RoleName", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("f089198c-ed4c-4294-9e62-ac9a09880000"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Quản trị viên", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("f089198c-ed4c-4294-9e62-ac9a09880001"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Quản lý", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("f089198c-ed4c-4294-9e62-ac9a09880002"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Giảng viên", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Topic",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "IsDeleted", "TopicName", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0a14f049-5555-4c9f-a834-d99b16176001"), new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, "인사", new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-5555-4c9f-a834-d99b16176002"), new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, "교통", new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-5555-4c9f-a834-d99b16176003"), new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, "과일", new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"), new DateTime(2025, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "LevelDetail",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsDeleted", "LevelId", "TopicId", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0a14f049-7777-8888-a834-d99b16176000"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new Guid("0a14f049-6666-4c9f-a834-d99b16176001"), new Guid("0a14f049-5555-4c9f-a834-d99b16176001"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-7777-8888-a834-d99b16176001"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new Guid("0a14f049-6666-4c9f-a834-d99b16176002"), new Guid("0a14f049-5555-4c9f-a834-d99b16176001"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-7777-8888-a834-d99b16176002"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new Guid("0a14f049-6666-4c9f-a834-d99b16176003"), new Guid("0a14f049-5555-4c9f-a834-d99b16176001"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-7777-8888-a834-d99b16176003"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new Guid("0a14f049-6666-4c9f-a834-d99b16176001"), new Guid("0a14f049-5555-4c9f-a834-d99b16176002"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-7777-8888-a834-d99b16176004"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new Guid("0a14f049-6666-4c9f-a834-d99b16176002"), new Guid("0a14f049-5555-4c9f-a834-d99b16176002"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-7777-8888-a834-d99b16176005"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new Guid("0a14f049-6666-4c9f-a834-d99b16176003"), new Guid("0a14f049-5555-4c9f-a834-d99b16176002"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-7777-8888-a834-d99b16176006"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new Guid("0a14f049-6666-4c9f-a834-d99b16176001"), new Guid("0a14f049-5555-4c9f-a834-d99b16176003"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-7777-8888-a834-d99b16176007"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new Guid("0a14f049-6666-4c9f-a834-d99b16176002"), new Guid("0a14f049-5555-4c9f-a834-d99b16176003"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0a14f049-7777-8888-a834-d99b16176008"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new Guid("0a14f049-6666-4c9f-a834-d99b16176003"), new Guid("0a14f049-5555-4c9f-a834-d99b16176003"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Avatar", "CreatedBy", "CreatedDate", "DateOfBirth", "Email", "FullName", "Gender", "IsActive", "IsDeleted", "Password", "PhoneNumber", "RoleId", "UpdatedBy", "UpdatedDate", "UserName" },
                values: new object[,]
                {
                    { new Guid("a089198c-ed4c-4294-9e62-ac9a09880000"), null, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateOnly(1990, 1, 1), "phuonght40@fpt.com", "Hoàng Thị Phương", true, true, false, "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9", "0123456789", new Guid("f089198c-ed4c-4294-9e62-ac9a09880000"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin" },
                    { new Guid("a089198c-ed4c-4294-9e62-ac9a09880001"), null, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateOnly(1992, 5, 20), "phongnt@gmail.com", "Nguyễn Thanh Phong", false, true, false, "5830aa9ba1fd7843c92fd956cb640604e6d3bff683ddeeac778e0af21089a303", "0987654321", new Guid("f089198c-ed4c-4294-9e62-ac9a09880001"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "teamlead" },
                    { new Guid("a089198c-ed4c-4294-9e62-ac9a09880002"), null, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateOnly(1995, 10, 15), "thanhxuan123@gmail.com", "Lê Thanh Xuân", true, true, false, "a4c3fcb625ccf255765afd5e3548839e8a2de6c587d7125dfba735dda69dbe22", "0112233445", new Guid("f089198c-ed4c-4294-9e62-ac9a09880002"), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "lecturer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddQuestionTask_AssignId",
                table: "AddQuestionTask",
                column: "AssignId");

            migrationBuilder.CreateIndex(
                name: "IX_AddQuestionTask_LevelDetailId",
                table: "AddQuestionTask",
                column: "LevelDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_AddQuestionTask_QuestionTypeId",
                table: "AddQuestionTask",
                column: "QuestionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_QuestionId",
                table: "Answer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConstraintDetail_ExamTypeConstraintId",
                table: "ConstraintDetail",
                column: "ExamTypeConstraintId");

            migrationBuilder.CreateIndex(
                name: "IX_ConstraintDetail_QuestionTypeId",
                table: "ConstraintDetail",
                column: "QuestionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ConstraintDetail_TopicId",
                table: "ConstraintDetail",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_ExamTypeId",
                table: "Exam",
                column: "ExamTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_ReviewerId",
                table: "Exam",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTypeConfiguration_LevelId",
                table: "ExamTypeConfiguration",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTypeConstraint_ExamTypeId",
                table: "ExamTypeConstraint",
                column: "ExamTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LevelDetail_LevelId",
                table: "LevelDetail",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_LevelDetail_TopicId",
                table: "LevelDetail",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Paper_ExamId",
                table: "Paper",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_PaperDetail_QuestionId",
                table: "PaperDetail",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_LevelDetailId",
                table: "Question",
                column: "LevelDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_LogId",
                table: "Question",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionTypeId",
                table: "Question",
                column: "QuestionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_ReferenceId",
                table: "Question",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_TaskId",
                table: "Question",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccessLog_UserId",
                table: "SystemAccessLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Avatar",
                table: "User",
                column: "Avatar",
                unique: true,
                filter: "[Avatar] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answer");

            migrationBuilder.DropTable(
                name: "ConstraintDetail");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PaperDetail");

            migrationBuilder.DropTable(
                name: "ExamTypeConstraint");

            migrationBuilder.DropTable(
                name: "Paper");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Exam");

            migrationBuilder.DropTable(
                name: "AddQuestionTask");

            migrationBuilder.DropTable(
                name: "References");

            migrationBuilder.DropTable(
                name: "SystemAccessLog");

            migrationBuilder.DropTable(
                name: "ExamTypeConfiguration");

            migrationBuilder.DropTable(
                name: "LevelDetail");

            migrationBuilder.DropTable(
                name: "QuestionType");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Level");

            migrationBuilder.DropTable(
                name: "Topic");

            migrationBuilder.DropTable(
                name: "ImageFile");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
