using AutoMapper;
using KEB.Application.DTOs.ReferenceDTO;
using KEB.Domain.Entities;
using KEB.Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Mappings
{
    public class ReferenceMapper : Profile
    {
        public ReferenceMapper()
        {
            CreateMap<References, ReferenceDisplayDto>()
                .ForMember(dest => dest.NumOfQuestions,
                opt => opt.MapFrom(src => src.Questions.Count(x => x.Status == QuestionStatus.Ok)));

            CreateMap<AddReferenceDto, References>()
                .ForMember(dest => dest.ReferenceName,
                opt => opt.MapFrom(src => src.ReferenceName.Trim()))
                .ForMember(dest => dest.ReferencesLink,
                opt => opt.MapFrom(src => src.ReferenceLink.Trim()))
                .ForMember(dest => dest.ReferenceAuthor,
                opt => opt.MapFrom(src => src.ReferenceAuthor.Trim()))
                .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Description.Trim()));

            CreateMap<UpdateReference, References>()
                .ForMember(dest => dest.ReferenceName,
                    opt => opt.MapFrom(src => src.ReferenceName.Trim()))
                .ForMember(dest => dest.ReferenceAuthor,
                    opt => opt.MapFrom(src => src.ReferenceAuthor.Trim()))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.Description.Trim()))
                .ForMember(dest => dest.ReferencesLink,
                    opt => opt.MapFrom(src => src.ReferenceLink.Trim()))
                .ForMember(dest => dest.PublishedYear,
                    opt => opt.MapFrom(src => src.PublishedYear));


        }

    }
}
