using KEB.Application.Services.Interfaces;
using KEB.Infrastructure.ExternalService.IExternalInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services
{
    public interface IUnitOfService
    {
        IAddQuestionTaskService AddQuestionTaskService { get; }
        IUserService UserService { get; }
        IAuthService CommonService { get; }
        IRoleService RoleService { get; }
        ITopicService TopicService { get; set; }
        ILevelService LevelService { get; }
        ILevelDetailService LevelDetailService { get; }
        IExamService ExamService { get; }
        IExamPaperService ExamPaperService { get; }
        IExamTypeService ExamTypeService { get; }
        IReferenceService ReferenceService { get; }
        IQuestionTypeService QuestionTypeService { get; }
        IQuestionService QuestionService { get; }
        IStatisticsService StatisticsService { get; }
        IAccessLogService AccessLogService { get; }
        IQuestionWithFileService QuestionWithFileService { get; }
        IEmailNotiService EmailNotiService { get; }
        INotiService NotiService { get; }
        //IImageFileService ImageFileService { get; }
    }
}
