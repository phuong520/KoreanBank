
using Hangfire;
using KEB.Infrastructure.Context;
using KEB.Infrastructure.ExternalService.IExternalImplementation;
using KEB.Infrastructure.ExternalService.IExternalInterfaces;
using KEB.Infrastructure.Repository.Implementations;
using KEB.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExamBankContext _context;
        private IDbContextTransaction _currentTransaction;
       // private readonly BlobServiceClient _blobServiceClient;

        public UnitOfWork(ExamBankContext context)
        {
            _context = context;
            //_blobServiceClient = blobServiceClient;
            Common = new CommonRepository(context);
            Users = new UserRepository(_context);
            Roles = new RoleRepository(_context);
            Levels = new LevelRepository(_context);
            Topics = new TopicRepository(_context);
            LevelDetails = new LevelDetailRepository(_context);
            References = new ReferenceRepository(_context);
            QuestionTypes = new QuestionTypeRepository(_context);
            AddQuestionTasks = new AddQuestionTaskRepository(_context);
            Exams = new ExamRepository(_context);
            Papers = new PaperRepository(_context);
            ExamTypes = new ExamTypeRepository(_context);
            ExamTypesConstraints = new ExamTypeConstraintRepository(_context);
            ConstraintDetails = new ConstraintDetailRepository(_context);
            Questions = new QuestionRepository(_context);
            AccessLogs = new SystemAccessLogRepository(_context);
            Answers = new AnswerRepository(_context);
            EmailService = new EmailService();
            //FileService = new FileService(blobServiceClient);
            ImageFiles = new ImageFileIRepository(_context);
            Notifications = new NotificationRepository(_context);
        }

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

        public IFileService FileService { get; }

        public INotificationRepository Notifications { get; }
        public IImageFileRepository ImageFiles { get; }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction is not null)
                throw new InvalidOperationException("A transaction has already been started.");
            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_currentTransaction is null)
                throw new InvalidOperationException("A transaction has not been started.");

            try
            {
                await _currentTransaction.CommitAsync();
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
            catch (Exception)
            {
                if (_currentTransaction is not null)
                {
                    await _currentTransaction.RollbackAsync();
                    _currentTransaction.Dispose();
                }
                throw;
            }
        }

        public bool DeleteScheduledJob(string jobId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
        {
            string jobId = BackgroundJob.Enqueue(methodCall);
            return jobId;
        }

        public async Task RollbackAsync()
        {
            await _currentTransaction.RollbackAsync();
            _currentTransaction.Dispose();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
        {
            string jobId = BackgroundJob.Schedule(methodCall, delay);
            return jobId;
        }

        public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
        {
            string jobId = BackgroundJob.Schedule(methodCall, delay);
            return jobId;
        }

        public string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt)
        {
            string jobId = BackgroundJob.Schedule(methodCall, enqueueAt);
            return jobId;
        }

        public string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt)
        {
            string jobId = BackgroundJob.Schedule(methodCall, enqueueAt);
            return jobId;
        }

        public string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt)
        {
            string jobId = BackgroundJob.Schedule(methodCall, enqueueAt);
            return jobId;
        }
    }
}
