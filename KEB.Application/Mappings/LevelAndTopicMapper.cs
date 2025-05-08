using AutoMapper;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.LevelTopicDetailDTO;
using KEB.Application.DTOs.TopicDTO;
using KEB.Domain.Entities;
using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Mappings
{
    public class LevelAndTopicMapper : Profile
    {
        public LevelAndTopicMapper() {
            CreateMap<IEnumerable<Question>, NumOfQuestionsBySkillDTO>()
        .ForMember(dest => dest.NumOfListeningQuestions,
        opt => opt.MapFrom(src => src.Count(x => x.Status == QuestionStatus.Ok && x.QuestionType.Skill == Skill.Nghe)))
        .ForMember(dest => dest.NumOfSpeakingQuestions,
        opt => opt.MapFrom(src => src.Count(x => x.Status == QuestionStatus.Ok && x.QuestionType.Skill == Skill.Nói)))
        .ForMember(dest => dest.NumOfReadingQuestions,
        opt => opt.MapFrom(src => src.Count(x => x.Status == QuestionStatus.Ok && x.QuestionType.Skill == Skill.Đọc)))
        .ForMember(dest => dest.NumOfWritingQuestions,
        opt => opt.MapFrom(src => src.Count(x => x.Status == QuestionStatus.Ok && x.QuestionType.Skill == Skill.Viết)));

            CreateMap<LevelDetail, NumOfQuestionsBySkillDTO>()
                .ForMember(dest => dest.NumOfListeningQuestions,
                opt => opt.MapFrom(src => src.Questions.Count(x => x.Status == QuestionStatus.Ok && x.QuestionType.Skill == Skill.Nghe)))
                .ForMember(dest => dest.NumOfSpeakingQuestions,
                opt => opt.MapFrom(src => src.Questions.Count(x => x.Status == QuestionStatus.Ok && x.QuestionType.Skill == Skill.Nói)))
                .ForMember(dest => dest.NumOfReadingQuestions,
                opt => opt.MapFrom(src => src.Questions.Count(x => x.Status == QuestionStatus.Ok && x.QuestionType.Skill == Skill.Đọc)))
                .ForMember(dest => dest.NumOfWritingQuestions,
                opt => opt.MapFrom(src => src.Questions.Count(x => x.Status == QuestionStatus.Ok && x.QuestionType.Skill == Skill.Viết)));

            CreateMap<IEnumerable<LevelDetail>, NumOfQuestionsBySkillDTO>()
                .ForMember(dest => dest.NumOfListeningQuestions,
                opt => opt.MapFrom(src => src.Sum(x => x.Questions.Count(x => x.Status == QuestionStatus.Ok && x.QuestionType.Skill == Skill.Nghe))))
                .ForMember(dest => dest.NumOfSpeakingQuestions,
                opt => opt.MapFrom(src => src.Sum(x => x.Questions.Count(x => x.Status == QuestionStatus.Ok && x.QuestionType.Skill == Skill.Nói))))
                .ForMember(dest => dest.NumOfReadingQuestions,
                opt => opt.MapFrom(src => src.Sum(x => x.Questions.Count(x => x.Status == QuestionStatus.Ok && x.QuestionType.Skill == Skill.Đọc))))
                .ForMember(dest => dest.NumOfWritingQuestions,
                opt => opt.MapFrom(src => src.Sum(x => x.Questions.Count(x => x.Status == QuestionStatus.Ok && x.QuestionType.Skill == Skill.Viết))));

            CreateMap<LevelDetail, TopicDisplayDto>()
                .ForMember(dest => dest.TopicId,
                opt => opt.MapFrom(src => src.TopicId))
                .ForMember(dest => dest.TopicName,
                opt => opt.MapFrom(src => src.Topic.TopicName))
                .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Topic.Description))
                .ForMember(dest => dest.NumOfQuestions,
                opt => opt.MapFrom(src => src));

            CreateMap<LevelDetail, LevelDisplayBriefDTO>()
                .ForMember(dest => dest.LevelId,
                opt => opt.MapFrom(src => src.LevelId))
                .ForMember(dest => dest.LevelName,
                opt => opt.MapFrom(src => src.Level.LevelName))
                .ForMember(dest => dest.NumOfQuestions,
                opt => opt.MapFrom(src => src));

            // Level
            CreateMap<Level, LevelDisplayBriefDTO>()
                .ForMember(dest => dest.LevelId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LevelName,
                opt => opt.MapFrom(src => src.LevelName))
                .ForMember(dest => dest.NumOfRelatedTopics,
                opt => opt.MapFrom(src => src.LevelDetails.Count))
                .ForMember(dest => dest.NumOfQuestions,
                opt => opt.MapFrom(src => src.LevelDetails));

            CreateMap<Level, LevelDisplayDetailDTO>()
                .ForMember(dest => dest.LevelId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LevelName,
                opt => opt.MapFrom(src => src.LevelName))
                .ForMember(dest => dest.Topics,
                opt => opt.MapFrom(src => src.LevelDetails))
                ;

            // Topic
            CreateMap<Topic, TopicDisplayDto>()
                .ForMember(dest => dest.TopicId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.NumOfRelatedLevels,
                opt => opt.MapFrom(src => src.LevelDetails.Count))
                .ForMember(dest => dest.NumOfQuestions,
                opt => opt.MapFrom(src => src.LevelDetails));

            CreateMap<Topic, TopicDetailDto>()
                .ForMember(dest => dest.TopicId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.TopicName,
                opt => opt.MapFrom(src => src.TopicName))
                .ForMember(dest => dest.Levels,
                opt => opt.MapFrom(src => src.LevelDetails))
                ;
            CreateMap<LevelDetail, DetailDisplayDTO>()
                .ForMember(dest => dest.DetailId,
                ld => ld.MapFrom(src => src.Id))
                .ForMember(dest => dest.TopicName,
                ld => ld.MapFrom(src => src.Topic.TopicName));
                
        }
    }
}
