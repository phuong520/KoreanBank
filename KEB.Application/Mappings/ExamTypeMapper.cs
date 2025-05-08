using AutoMapper;
using KEB.Application.DTOs.ExamTypeConstraintDTO;
using KEB.Application.DTOs.ExamTypeDTO;
using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Mappings
{
    public class ExamTypeMapper: Profile
    {
        public ExamTypeMapper()
        {
            CreateMap<ExamType, ExamTypeGeneralDisplayDTO>()
                .ForMember(dest => dest.OccuredExams,
                opt => opt.MapFrom(src => src.Exams.Count()))
                .ForMember(dest => dest.LevelName,
                opt => opt.MapFrom(src => src.Levels.LevelName))
                .ForMember(dest => dest.ExamTypeId,
                opt => opt.MapFrom(src => src.Id));

            CreateMap<ExamType, ExamTypeComplexDisplayDTO>()
                .ForMember(dest => dest.OccuredExams,
                opt => opt.MapFrom(src => src.Exams.Count()))
                .ForMember(dest => dest.ExamTypeId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LevelId,
                opt => opt.MapFrom(src => src.LevelId))
                .ForMember(dest => dest.LevelName,
                opt => opt.MapFrom(src => src.Levels.LevelName))
                .ForMember(dest => dest.PaperConstraints,
                opt => opt.MapFrom(src => src.ExamTypeConstraints));

            CreateMap<ExamTypeConstraint, ConstraintToBeDisplayedDTO>()
                .ForMember(dest => dest.ExamTypeConstraintId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ConstraintDetails,
                opt => opt.MapFrom(src => src.ConstraintDetails));

            CreateMap<ConstraintDetail, ConstraintDetailToBeDisplayedDTO>()
                .ForMember(dest => dest.ConstraintDetailId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuestionTypeId,
                opt => opt.MapFrom(src => src.QuestionTypeId))
                .ForMember(dest => dest.QuestionTypeName,
                opt => opt.MapFrom(src => src.QuestionType.TypeName))
                .ForMember(dest => dest.TopicId,
                opt => opt.MapFrom(src => src.TopicId))
                .ForMember(dest => dest.TopicName,
                opt => opt.MapFrom(src => src.Topic.TopicName))
                .ForMember(dest => dest.NumOfQuestions,
                opt => opt.MapFrom(src => src.NumberOfQuestion))
                .ForMember(dest => dest.MarkPerQuestion,
                opt => opt.MapFrom(src => src.MarkPerQuestion))
                .ForMember(dest => dest.TotalMark,
                opt => opt.MapFrom(src => src.NumberOfQuestion * src.MarkPerQuestion))
                .ForMember(dest => dest.QuestionForm,
                opt => opt.MapFrom(src => src.IsMultipleChoice ? "Trắc nghiệm" : "Tự luận"))
                .ForMember(dest => dest.Difficulty,
                opt => opt.MapFrom(src => src.Difficulty.ToString()));
        }

    }
}
