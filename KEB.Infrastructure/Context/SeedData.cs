using KEB.Domain.Entities;
using KEB.Domain.Enums;
using KEB.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Context
{

    public class SeedData
    {
        private static readonly string adminHashed = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("admin123"))).Replace("-", "").ToLower();
        private static readonly string leadHashed = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("lead123"))).Replace("-", "").ToLower();
        private static readonly string lecturerHashed = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("lecturer123"))).Replace("-", "").ToLower();

        public static void Seed(ModelBuilder modelBuilder)
        {
            //Role
            modelBuilder.Entity<Role>().HasData(
               new Role
               {
                   Id = new Guid("f089198c-ed4c-4294-9e62-ac9a09880000"),
                   RoleName = "Quản trị viên"
               },
               new Role
               {
                   Id = new Guid("f089198c-ed4c-4294-9e62-ac9a09880001"),
                   RoleName = "Quản lý"
               },
               new Role
               {
                   Id = new Guid("f089198c-ed4c-4294-9e62-ac9a09880002"),
                   RoleName = "Giảng viên"
               }
           );
            //user
            
        modelBuilder.Entity<User>().HasData(
    new User
    {
        Id = new Guid("a089198c-ed4c-4294-9e62-ac9a09880000"),
        FullName = "Hoàng Thị Phương",
        Email = "phuongminzy2@gmail.com",
        Gender = true,
        Avatar = Guid.Parse("25E6A826-7D08-476B-B0BC-DC832E20D66F"),
        UserName = "admin",
        Password = adminHashed,
        DateOfBirth = new DateOnly(1990, 1, 1),
        PhoneNumber = "0123456789",
        IsActive = true,
        CreatedBy = Guid.Empty,
        UpdatedBy = Guid.Empty,
        RoleId = new Guid(LogicString.Role.AdminRoleId), // Admin
        NotificationId = Guid.NewGuid()
    },
    new User
    {
        Id = new Guid("a089198c-ed4c-4294-9e62-ac9a09880001"),
        FullName = "Nguyễn Thanh Phong",
        Email = "thanhphongh5201314@gmail.com",
        Gender = false,
        Avatar = Guid.Parse("F06C7D54-653F-4DE7-A588-31C632178194"),
        UserName = "teamlead",
        Password = leadHashed,
        DateOfBirth = new DateOnly(1992, 5, 20),
        PhoneNumber = "0987654321",
        IsActive = true,
        CreatedBy = Guid.Empty,
        UpdatedBy = Guid.Empty,
        RoleId = new Guid(LogicString.Role.TeamLeadRoleId), // Team Lead
        NotificationId = Guid.NewGuid()
    },
    new User
    {
        Id = new Guid("a089198c-ed4c-4294-9e62-ac9a09880002"),
        FullName = "Lê Thanh Xuân",
        Email = "blueberry5201314@gmail.com",
        Gender = true,
        Avatar = Guid.Parse("0C62C57C-A15F-43A2-8829-FF6E36009719"),
        UserName = "lecturer",
        Password = lecturerHashed,
        DateOfBirth = new DateOnly(1995, 10, 15),
        PhoneNumber = "0112233445",
        IsActive = true,
        CreatedBy = Guid.Empty,
        UpdatedBy = Guid.Empty,
        RoleId = new Guid(LogicString.Role.LecturerRoleId), // Lecturer
        NotificationId = Guid.NewGuid()
    }
);
            //question type
            modelBuilder.Entity<QuestionType>().HasData(
                new QuestionType()
                {
                    Id = new Guid("0a14f049-4da1-4c9f-a834-d99b16176f50"),
                    TypeName = "Nghe và trả lời câu hỏi",
                    TypeContent = "다음을 듣고 질문을 답하십시오",
                    Skill = Skill.Nghe, // Nghe
                    CreatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"),
                    CreatedDate = new DateTime(2025, 3, 17),
                    UpdatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"),
                    UpdatedDate = new DateTime(2025, 3, 17),
                },
                new QuestionType()
                {
                    Id = new Guid("0a14f049-4da1-4c9f-a834-d99b16176f52"),
                    TypeName   = "Tự sự về bản thân",
                    TypeContent = "다음을 듣고 알맟은 것을 고르십시오",
                    Skill = Skill.Nói, // Nói
                    CreatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"),
                    CreatedDate = new DateTime(2024, 9, 17),
                    UpdatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"),
                    UpdatedDate = new DateTime(2024, 9, 17),
                },
                new QuestionType()
                {
                    Id = new Guid("0a14f049-4da1-4c9f-a834-d99b16176f53"),
                    TypeName = "Chọn ngữ pháp đúng điền vào chỗ trống",
                    TypeContent = "다음을 듣고 알맟은 것을 고르십시오",
                    Skill = Skill.Đọc, // Đọc
                    CreatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"),
                    CreatedDate = new DateTime(2025, 3, 17),
                    UpdatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"),
                    UpdatedDate = new DateTime(2025, 3, 17),
                },
                new QuestionType()
                {
                    Id = new Guid("0a14f049-4da1-4c9f-a834-d99b16176f54"),
                    TypeName = "Viết câu 51,52",
                    TypeContent = "다음을 듣고 알맟은 것을 고르십시오",
                    Skill = Skill.Viết, // Viết
                    CreatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"),
                    CreatedDate = new DateTime(2025, 3, 17),
                    UpdatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a24072002"),
                    UpdatedDate = new DateTime(2025, 3, 17),
                }
            );
            //reference
            modelBuilder.Entity<References>().HasData(
                new References
                {
                    Id = new Guid("0a14f049-4444-4c9f-a834-d99b16176001"),
                    CreatedDate = new DateTime(2025, 3, 22),
                    UpdatedDate = new DateTime(2025, 3, 22),
                    CreatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
                    UpdatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
                    PublishedYear = 2002,
                    ReferenceAuthor = "No info",
                    ReferencesLink = "No info",
                    ReferenceName = "Tiếng Hàn tổng hợp quyển 1",
                    Description = "No info"
                },
                new References
                {
                    Id = new Guid("0a14f049-4444-4c9f-a834-d99b16176002"),
                    CreatedDate = new DateTime(2025, 3, 22),
                    UpdatedDate = new DateTime(2025, 3, 22),
                    CreatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
                    UpdatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
                    PublishedYear = 2002,
                    IsDeleted = false,
                    ReferenceAuthor = "No info",
                    ReferencesLink = "No info",
                    ReferenceName = "Tiếng Hàn tổng hợp quyển 2",
                    Description = "No info"
                },
                new References
                {
                    Id = new Guid("0a14f049-4444-4c9f-a834-d99b16176000"),
                    CreatedDate = new DateTime(2025, 3, 22),
                    UpdatedDate = new DateTime(2025, 3, 22),
                    CreatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
                    UpdatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
                    PublishedYear = 2002,
                    IsDeleted = false,
                    ReferenceAuthor = "No info",
                    ReferencesLink = "No info",
                    ReferenceName = "Tài liệu khác",
                    Description = "No info"
                }
            );
            //topic
            modelBuilder.Entity<Topic>().HasData(
               new Topic
               {
                   Id = new Guid("0a14f049-5555-4c9f-a834-d99b16176001"),
                   TopicName = "인사",
                   Description = "",
                   CreatedDate = new DateTime(2025, 3, 22),
                   UpdatedDate = new DateTime(2025, 3, 22),
                   CreatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
                   UpdatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
               },
               new Topic
               {
                   Id = new Guid("0a14f049-5555-4c9f-a834-d99b16176002"),
                   TopicName = "교통",
                   Description = "",
                   CreatedDate = new DateTime(2025, 3, 22),
                   UpdatedDate = new DateTime(2025, 3, 22),
                   CreatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
                   UpdatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
               },
               new Topic
               {
                   Id = new Guid("0a14f049-5555-4c9f-a834-d99b16176003"),
                   TopicName = "과일",
                   Description = "",
                   CreatedDate = new DateTime(2025, 3, 22),
                   UpdatedDate = new DateTime(2025, 3, 22),
                   CreatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
                   UpdatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
               }
           );
            //level
            modelBuilder.Entity<Level>().HasData(
              new Level
              {
                  Id = new Guid("0a14f049-6666-4c9f-a834-d99b16176001"),
                  LevelName = "Sơ cấp 1",
                  CreatedDate = new DateTime(2025, 3, 22),
                  UpdatedDate = new DateTime(2025, 3, 22),
                  CreatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
                  UpdatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
              },
              new Level
              {
                  Id = new Guid("0a14f049-6666-4c9f-a834-d99b16176002"),
                  LevelName = "Sơ cấp 2",
                  CreatedDate = new DateTime(2025, 3, 22),
                  UpdatedDate = new DateTime(2025, 3, 22),
                  CreatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
                  UpdatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
              },
              new Level
              {
                  Id = new Guid("0a14f049-6666-4c9f-a834-d99b16176003"),
                  LevelName = "Trung cấp 3",
                  CreatedDate = new DateTime(2025, 3, 22),
                  UpdatedDate = new DateTime(2025, 3, 22),
                  CreatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
                  UpdatedBy = new Guid("0000198c-ed4c-4294-9e62-ac9a02012003"),
              }
          );
            //level detail
            modelBuilder.Entity<LevelDetail>().HasData(
               new LevelDetail
               {
                   Id = new Guid("0a14f049-7777-8888-a834-d99b16176000"),
                   LevelId = new Guid("0a14f049-6666-4c9f-a834-d99b16176001"),
                   TopicId = new Guid("0a14f049-5555-4c9f-a834-d99b16176001")
               },
               new LevelDetail
               {
                   Id = new Guid("0a14f049-7777-8888-a834-d99b16176001"),
                   LevelId = new Guid("0a14f049-6666-4c9f-a834-d99b16176002"),
                   TopicId = new Guid("0a14f049-5555-4c9f-a834-d99b16176001")
               },
               new LevelDetail
               {
                   Id = new Guid("0a14f049-7777-8888-a834-d99b16176002"),
                   LevelId = new Guid("0a14f049-6666-4c9f-a834-d99b16176003"),
                   TopicId = new Guid("0a14f049-5555-4c9f-a834-d99b16176001")
               },
               new LevelDetail
               {
                   Id = new Guid("0a14f049-7777-8888-a834-d99b16176003"),
                   LevelId = new Guid("0a14f049-6666-4c9f-a834-d99b16176001"),
                   TopicId = new Guid("0a14f049-5555-4c9f-a834-d99b16176002")
               },
               new LevelDetail
               {
                   Id = new Guid("0a14f049-7777-8888-a834-d99b16176004"),
                   LevelId = new Guid("0a14f049-6666-4c9f-a834-d99b16176002"),
                   TopicId = new Guid("0a14f049-5555-4c9f-a834-d99b16176002")
               },
               new LevelDetail
               {
                   Id = new Guid("0a14f049-7777-8888-a834-d99b16176005"),
                   LevelId = new Guid("0a14f049-6666-4c9f-a834-d99b16176003"),
                   TopicId = new Guid("0a14f049-5555-4c9f-a834-d99b16176002")
               },
               new LevelDetail
               {
                   Id = new Guid("0a14f049-7777-8888-a834-d99b16176006"),
                   LevelId = new Guid("0a14f049-6666-4c9f-a834-d99b16176001"),
                   TopicId = new Guid("0a14f049-5555-4c9f-a834-d99b16176003")
               },
               new LevelDetail
               {
                   Id = new Guid("0a14f049-7777-8888-a834-d99b16176007"),
                   LevelId = new Guid("0a14f049-6666-4c9f-a834-d99b16176002"),
                   TopicId = new Guid("0a14f049-5555-4c9f-a834-d99b16176003")
               },
               new LevelDetail
               {
                   Id = new Guid("0a14f049-7777-8888-a834-d99b16176008"),
                   LevelId = new Guid("0a14f049-6666-4c9f-a834-d99b16176003"),
                   TopicId = new Guid("0a14f049-5555-4c9f-a834-d99b16176003")
               }
           );
        }
    }
}
