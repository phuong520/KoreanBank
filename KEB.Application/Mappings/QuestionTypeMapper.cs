using AutoMapper;
using KEB.Application.DTOs.QuestionTypeDTO;
using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Mappings
{
    internal class QuestionTypeMapper:Profile
    {
        public QuestionTypeMapper()
        {
            CreateMap<QuestionTypeCreateDto, QuestionType>()
           .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.QuestionTypeName))
           .ForMember(dest => dest.TypeContent, opt => opt.MapFrom(src => src.QuestionTypeContent))
           .ForMember(dest => dest.Skill, opt => opt.MapFrom(src => src.Skill)); // Ánh xạ các thuộc tính tương ứng

            CreateMap<QuestionType, QuestionTypeDisplayDto>()
                .ForMember(dest => dest.QuestionTypeId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuestionTypeName, opt => opt.MapFrom(src => src.TypeName))
                .ForMember(dest => dest.QuestionTypeContent, opt => opt.MapFrom(src => src.TypeContent))
                .ForMember(dest => dest.Skill, opt => opt.MapFrom(src => src.Skill))
                .ForMember(dest => dest.NumOfQuestions, opt => opt.MapFrom(src => src.Questions.Count(x => x.Status == Domain.Enums.QuestionStatus.Ok)));
        }
    }
}
