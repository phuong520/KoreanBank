using AutoMapper;
using KEB.Application.DTOs.ExamDTO;
using KEB.Application.DTOs.ExamPaperDTO;
using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Mappings
{
    public class ExamAndPaperMapper :Profile
    {
        public ExamAndPaperMapper()
        {
            CreateMap<Exam, ExamGeneralDisplayDTO>()
                .ForMember(dest => dest.ExamId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Occured,
                opt => opt.MapFrom(src => src.TakePlaceTime < DateTime.Now ? "Đã thi" : "Chưa thi"))
                .ForMember(dest => dest.ExamTypeName,
                opt => opt.MapFrom(src => src.ExamType.ExamTypeName))
                .ForMember(dest => dest.LevelName,
                opt => opt.MapFrom(src => src.ExamType.Levels.LevelName));

            CreateMap<Exam, ExamComplexDisplayDTO>()
                .ForMember(dest => dest.ExamId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ExamTypeName,
                opt => opt.MapFrom(src => src.ExamType.ExamTypeName))
                .ForMember(dest => dest.LevelName,
                opt => opt.MapFrom(src => src.ExamType.Levels.LevelName))
                .ForMember(dest => dest.HostUserName,
                opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.ReviewerUserName,
                opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.TakePlaceTime,
                opt => opt.MapFrom(src => src.TakePlaceTime));

            CreateMap<Exam, ExamAsTaskDisplayDTO>()
                .ForMember(dest => dest.ExamId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ExamName,
                opt => opt.MapFrom(src => src.ExamName))
                .ForMember(dest => dest.TakePlaceTime,
                opt => opt.MapFrom(src => src.TakePlaceTime))
                .ForMember(dest => dest.Occured,
                opt => opt.MapFrom(src => src.TakePlaceTime > DateTime.Now));

            CreateMap<Paper, PaperGeneralDisplayDTO>()
                .ForMember(dest => dest.ExamName,
                opt => opt.MapFrom(src => src.Exam.ExamName))
                .ForMember(dest => dest.Skill,
                opt => opt.MapFrom(src => src.Skill))
                .ForMember(dest => dest.TakePlaceTime,
                opt => opt.MapFrom(src => src.Exam.TakePlaceTime))
                .ForMember(dest => dest.LevelName,
                opt => opt.MapFrom(src => src.Exam.ExamType.Levels.LevelName));

            CreateMap<Paper, PaperDetailDisplayDTO>()
                .ForMember(dest => dest.PaperId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PaperName,
                opt => opt.MapFrom(src => src.PaperName))
                .ForMember(dest => dest.ExamId,
                opt => opt.MapFrom(src => src.ExamId))
                .ForMember(dest => dest.ExamName,
                opt => opt.MapFrom(src => src.Exam.ExamName))
                .ForMember(dest => dest.TakePlaceTime,
                opt => opt.MapFrom(src => src.Exam.TakePlaceTime))
                .ForMember(dest => dest.LevelName,
                opt => opt.MapFrom(src => src.Exam.ExamType.Levels.LevelName))
                .ForMember(dest => dest.QuestionsList,
                opt => opt.MapFrom(src => src.PaperDetails));

            CreateMap<PaperDetail, QuestionInPaperDTO>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Question.Id))
                .ForMember(dest => dest.QuestionTypeId,
                opt => opt.MapFrom(src => src.Question.QuestionType.Id))
                .ForMember(dest => dest.QuestionTypeName,
                opt => opt.MapFrom(src => src.Question.QuestionType.TypeName))
                .ForMember(dest => dest.QuestionTypeContent,
                opt => opt.MapFrom(src => src.Question.QuestionType.TypeName))
                .ForMember(dest => dest.Skill,
                opt => opt.MapFrom(src => src.Question.QuestionType.Skill))
                .ForMember(dest => dest.QuestionContent,
                opt => opt.MapFrom(src => src.Question.QuestionContent))
                .ForMember(dest => dest.Answers,
                opt => opt.MapFrom(src => src.Question.Answers))
                .ForMember(dest => dest.LevelName,
                opt => opt.MapFrom(src => src.Question.LevelDetail.Level.LevelName))
                .ForMember(dest => dest.TopicName,
                opt => opt.MapFrom(src => src.Question.LevelDetail.Topic.TopicName))
                .ForMember(dest => dest.ReferenceName,
                opt => opt.MapFrom(src => src.Question.References.ReferenceName))
                .ForMember(dest => dest.Difficulty,
                opt => opt.MapFrom(src => src.Question.Difficulty.ToString()))
                .ForMember(dest => dest.AttachmentUrl,
                opt => opt.MapFrom(src => src.Attachment))
                .ForMember(dest => dest.Mark,
                opt => opt.MapFrom(src => src.Mark))
                .ForMember(dest => dest.OrderInPaper,
                opt => opt.MapFrom(src => src.OrderInPaper));

            CreateMap<Question, QuestionInPaperDTO>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuestionTypeId,
                opt => opt.MapFrom(src => src.QuestionTypeId))
                .ForMember(dest => dest.QuestionTypeName,
                opt => opt.MapFrom(src => src.QuestionType.TypeName))
                .ForMember(dest => dest.QuestionTypeContent,
                opt => opt.MapFrom(src => src.QuestionType.TypeName))
                .ForMember(dest => dest.Skill,
                opt => opt.MapFrom(src => src.QuestionType.Skill))
                .ForMember(dest => dest.Answers,
                opt => opt.MapFrom(src => src.Answers))
                .ForMember(dest => dest.LevelName,
                opt => opt.MapFrom(src => src.LevelDetail.Level.LevelName))
                .ForMember(dest => dest.TopicName,
                opt => opt.MapFrom(src => src.LevelDetail.Topic.TopicName))
                .ForMember(dest => dest.ReferenceName,
                opt => opt.MapFrom(src => src.References.ReferenceName))
                .ForMember(dest => dest.Difficulty,
                opt => opt.MapFrom(src => src.Difficulty.ToString()))
                .ForMember(dest => dest.AttachmentUrl,
                opt => opt.MapFrom(src => src.AttachmentFile));
        }

    }
}
