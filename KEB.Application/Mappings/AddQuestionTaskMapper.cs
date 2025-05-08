using AutoMapper;
using KEB.Application.DTOs.ImportQuestionTaskDTO;
using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Mappings
{
    public class AddQuestionTaskMapper : Profile
    {
        public AddQuestionTaskMapper()
        {
            CreateMap<AddQuestionTask, TaskGeneralDisplayDTO>()
                .ForMember(dest => dest.AssigneeName,
                opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.AssigneeId,
                opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.NumberOfAddedQuestions,
                opt => opt.MapFrom(src => src.Questions.Count()))
                .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.QuestionTypeId,
                opt => opt.MapFrom(src => src.QuestionTypeId))
                .ForMember(dest => dest.Skill,
                opt => opt.MapFrom(src => src.QuestionType.Skill))
                .ForMember(dest => dest.QuestionTypeName,
                opt => opt.MapFrom(src => src.QuestionType.TypeName))
                .ForMember(dest => dest.LevelDetailId,
                opt => opt.MapFrom(src => src.LevelDetailId))
                .ForMember(dest => dest.LevelDetail,
                opt => opt.MapFrom(src => $"{src.LevelDetail.Level.LevelName} - {src.LevelDetail.Topic.TopicName}"));

            CreateMap<AddQuestionTask, TaskFullDisplayDTO>()
                .ForMember(dest => dest.Skill,
                opt => opt.MapFrom(src => src.QuestionType.Skill))
                .ForMember(dest => dest.ForMultipleChoice,
                opt => opt.MapFrom(src => src.ForMultiChoice))
                .ForMember(dest => dest.AssigneeId,
                opt => opt.MapFrom(src => src.AssignId))
                .ForMember(dest => dest.AssigneeName,
                opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.QuestionTypeId,
                opt => opt.MapFrom(src => src.QuestionTypeId))
                .ForMember(dest => dest.QuestionTypeName,
                opt => opt.MapFrom(src => src.QuestionType.TypeName))
                .ForMember(dest => dest.LevelDetailId,
                opt => opt.MapFrom(src => src.LevelDetailId))
                .ForMember(dest => dest.LevelDetail,
                opt => opt.MapFrom(src => $"{src.LevelDetail.Level.LevelName} - {src.LevelDetail.Topic.TopicName}"));

        }
    }
}
