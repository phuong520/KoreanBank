using KEB.Domain.Entities;
using KEB.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Context
{
    public class ExamBankContext : DbContext
    {
        public ExamBankContext(DbContextOptions options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking; //
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AddQuestionTask> AddQuestions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<ConstraintDetail> ConstraintDetails { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamType> ExamTypes { get; set; }
        public DbSet<ExamTypeConstraint> ExamTypeConstraints { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<LevelDetail> LevelDetails { get; set; }
        public DbSet<Notification> NotificationDetails { get; set; }
        public DbSet<Paper> Papers { get; set; }
        public DbSet<PaperDetail> PaperDetails { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<References> References { get; set; }
        public DbSet<SystemAccessLog> SystemAccessLogs { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<ImageFile> ImageFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExamBankContext).Assembly);
            SeedData.Seed(modelBuilder);
        }
    }
}
