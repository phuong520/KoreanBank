using AutoMapper;
using KEB.Application.Services.Implementations;
using KEB.Application.Services.Interfaces;
using KEB.Infrastructure.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services
{
    public class UnitOfService : IUnitOfService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        private readonly ILogger<UnitOfService> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly HttpClient _httpClient;

        public UnitOfService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config, ILogger<UnitOfService> logger, ILoggerFactory loggerFactory, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
            _logger = logger;
            _loggerFactory = loggerFactory;
            _httpClient = httpClient;
            UserService = new UserService(unitOfWork, mapper);
            CommonService = new AuthService(unitOfWork, mapper,config);
            RoleService = new RoleService(unitOfWork, mapper);
            TopicService = new TopicService(unitOfWork, mapper);
            AddQuestionTaskService = new AddQuestionTaskService(unitOfWork, mapper, _config);
            LevelService = new LevelService(unitOfWork,mapper);
            LevelDetailService = new LevelDetailService(unitOfWork, mapper);
            ReferenceService = new ReferenceService(_unitOfWork, _mapper);
            ExamService = new ExamService(_unitOfWork, _mapper);
            ExamPaperService = new ExamPaperService(_unitOfWork, _mapper);
            ExamTypeService = new ExamTypeService(_unitOfWork, _mapper);
            QuestionTypeService = new QuestionTypeService(_unitOfWork, _mapper);
            AccessLogService = new AccessLogService(_unitOfWork, _mapper);
            StatisticsService = new StatisticsService(_unitOfWork);
            QuestionService = new QuestionService(_unitOfWork, _mapper);
           // FileTemplateService = new QuestionWithFileService(_unitOfWork, _mapper);
            EmailNotiService = new EmailNotiService(_unitOfWork);
            NotiService = new NotiService(_unitOfWork);
            QuestionWithFileService = new QuestionWithFileService(_unitOfWork, _mapper);
            //ImageFileService = new ImageFileService(_unitOfWork);
        }

        public IUserService UserService { get; set; }

        public IAuthService CommonService { get; set; }

        public IRoleService RoleService { get; set; }
        public ITopicService TopicService { get; set; }

        public IAddQuestionTaskService AddQuestionTaskService { get; set; }

        public ILevelService LevelService { get; set; }

        public ILevelDetailService LevelDetailService { get; set; }

        public IExamService ExamService { get; set; }

        public IExamPaperService ExamPaperService { get; set; }

        public IExamTypeService ExamTypeService { get; set; }

        public IReferenceService ReferenceService { get; set; }

        public IQuestionTypeService QuestionTypeService { get; set; }

        public IQuestionService QuestionService { get; set; }

        public IStatisticsService StatisticsService { get; set; }

        public IAccessLogService AccessLogService { get; set; }

        public IQuestionWithFileService FileTemplateService { get; set; }

        public IEmailNotiService EmailNotiService { get; set; }

        public INotiService NotiService { get; set; }
        public IQuestionWithFileService QuestionWithFileService { get; set; }
        //public IImageFileService ImageFileService { get; set; }

    }
}
