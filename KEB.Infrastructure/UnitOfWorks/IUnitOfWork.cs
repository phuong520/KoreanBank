using KEB.Infrastructure.Context;
using KEB.Infrastructure.ExternalService.IExternalInterfaces;
using KEB.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.UnitOfWorks
{

    public interface IUnitOfWork
    {
        public ICommonRepository Common { get; }
        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public ILevelRepository Levels { get; }
        public ITopicRepository Topics { get; }
        public ILevelDetailRepository LevelDetails { get; }
        public IReferenceRepository References { get; }
        public IQuestionTypeRepository QuestionTypes { get; }
        public IAddQuestionTaskRepository AddQuestionTasks { get; }
        public IExamRepository Exams { get; }
        public IPaperRepository Papers { get; }
        public IExamTypeRepository ExamTypes { get; }
        public IExamTypeConstraintRepository ExamTypesConstraints { get; }
        public IConstraintDetailRepository ConstraintDetails { get; }
        public IQuestionRepository Questions { get; }
        public ISystemAccessLogRepository AccessLogs { get; }
        public IAnswerRepository Answers { get; }
        public IEmailService EmailService { get; }
        //public IFileService FileService { get; }
        public IImageFileRepository ImageFiles { get; }
        public INotificationRepository Notifications { get; }
        public Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        //Commit the database Transaction
        Task CommitAsync();
        //Rollback the database Transaction
        Task RollbackAsync();


        void Dispose();
        public string Enqueue<T>(Expression<Func<T, Task>> methodCall);
        public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);
        public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);
        public string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt);
        public string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt);
        string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt);
        bool DeleteScheduledJob(string jobId);
    }
}
