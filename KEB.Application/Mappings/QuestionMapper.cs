using AutoMapper;
using KEB.Application.DTOs.AnswerDTO;
using KEB.Application.DTOs.QuestionAddDTO;
using KEB.Application.DTOs.QuestionDTO;
using KEB.Domain.Entities;
using KEB.Domain.Enums;

namespace KEB.Application.Mappings
{
    public class QuestionMapper : Profile
    {
        public QuestionMapper()
        {
            CreateMap<(object? AddedQuestion, List<string> Message), ImportQuestionResultDTO>()
                .ForMember(dest => dest.Question,
                opt => opt.MapFrom(src => src.AddedQuestion))
                .ForMember(dest => dest.Messages,
                opt => opt.MapFrom(src => src.Message));

            CreateMap<Question, QuestionDisplayDto>()
                .ForMember(dest => dest.AttachmentImage,
                opt => opt.MapFrom(src => Convert.ToBase64String(src.AttachmentFileImage.FileData)))
                .ForMember(dest => dest.AttachmentAudio,
                opt => opt.MapFrom(src => Convert.ToBase64String(src.AttachmentFileAudio.FileData)))
                .ForMember(dest => dest.QuestionTypeId,
                opt => opt.MapFrom(src => src.QuestionType.Id))
                .ForMember(dest => dest.QuestionTypeName,
                opt => opt.MapFrom(src => src.QuestionType.TypeName))
                .ForMember(dest => dest.SkillName,
                opt => opt.MapFrom(src => src.QuestionType.Skill.ToString()))
                .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.QuestionContent,
                opt => opt.MapFrom(src => src.QuestionContent))
                .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.LevelName,
                opt => opt.MapFrom(src => src.LevelDetail.Level.LevelName))
                .ForMember(dest => dest.TopicName,
                opt => opt.MapFrom(src => src.LevelDetail.Topic.TopicName))
                .ForMember(dest => dest.ReferenceName,
                opt => opt.MapFrom(src => src.References.ReferenceName))
                .ForMember(dest => dest.NotifyTo,
                opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => QuestionStatus.Ok))
                .ForMember(dest => dest.LogId,
                opt => opt.MapFrom(src => src.LogId));


            CreateMap<Question, QuestionDetailDto>()
                 .ForMember(dest => dest.Description,
                     opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.QuestionTypeId,
                     opt => opt.MapFrom(src => src.QuestionTypeId))
                 .ForMember(dest => dest.QuestionTypeName,
                     opt => opt.MapFrom(src => src.QuestionType.TypeName))
                 .ForMember(dest => dest.AttachmentImage,
                      opt => opt.MapFrom(src => Convert.ToBase64String(src.AttachmentFileImage.FileData)))//
                 .ForMember(dest => dest.AttachmentAudio,
                      opt => opt.MapFrom(src => Convert.ToBase64String(src.AttachmentFileAudio.FileData)))//
                 .ForMember(dest => dest.SkillName,
                     opt => opt.MapFrom(src => src.QuestionType.Skill.ToString())) 
                 .ForMember(dest => dest.Status,
                     opt => opt.MapFrom(src => src.Status.ToString())) 
                 .ForMember(dest => dest.LevelId,
                     opt => opt.MapFrom(src => src.LevelDetail.LevelId))
                 .ForMember(dest => dest.LevelName,
                     opt => opt.MapFrom(src => src.LevelDetail.Level.LevelName))
                 .ForMember(dest => dest.TopicName,
                     opt => opt.MapFrom(src => src.LevelDetail.Topic.TopicName))
                 .ForMember(dest => dest.TopicId,
                     opt => opt.MapFrom(src => src.LevelDetail.TopicId))
                 .ForMember(dest => dest.ReferenceName,
                     opt => opt.MapFrom(src => src.References.ReferenceName))
                 .ForMember(dest => dest.ReferenceId,
                     opt => opt.MapFrom(src => src.ReferenceId))
                 .ForMember(dest => dest.Difficulty,
                     opt => opt.MapFrom(src => src.Difficulty.ToString()));

            CreateMap<AddSingleQuestionRequest, Question>()
                .ForMember(dest => dest.LevelDetailId, opt => opt.MapFrom(src => src.LevelDetailId))
                .ForMember(dest => dest.ReferenceId, opt => opt.MapFrom(src => src.ReferenceId))
                .ForMember(dest => dest.QuestionTypeId, opt => opt.MapFrom(src => src.QuestionTypeId))
                .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Difficulty))
                .ForMember(dest => dest.QuestionContent, opt => opt.MapFrom(src => src.QuestionContent))
                .ForMember(dest => dest.Answers, opt => opt.Ignore())
                //.ForMember(dest => dest.Answers, opt => opt.MapFrom(src=> src.Answers))
                .ForMember(dest => dest.AttachmentDuration, opt => opt.MapFrom(src => src.AttachmentDuration))
                .ForMember(dest => dest.AttachmentFileImage, opt => opt.Ignore())
                .ForMember(dest => dest.AttachmentFileAudio, opt => opt.Ignore())
                .ForMember(dest => dest.IsMultipleChoice, opt => opt.MapFrom(src => src.IsMultipleChoice))
                .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.TaskId));

            CreateMap<AddAnswerDTO, Answer>();

            CreateMap<PaperDetail, QuestionDisplayDto>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Question.Id))
           .ForMember(dest => dest.QuestionContent, opt => opt.MapFrom(src => src.Question.QuestionContent))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Question.Status.ToString()))
           .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Question.References.ReferenceName)) // Hoặc ánh xạ theo cách khác
           .ForMember(dest => dest.LevelName, opt => opt.MapFrom(src => src.Question.LevelDetail.Level.LevelName)) // Tên Level
           .ForMember(dest => dest.ReferenceName, opt => opt.MapFrom(src => src.Question.References.ReferenceName)) // Ánh xạ tên Reference
           .ForMember(dest => dest.QuestionTypeName, opt => opt.MapFrom(src => src.Question.QuestionType.TypeName))
           //.ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Question.Skill.ToString()))
           .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Question.Difficulty.ToString())) // Hoặc định dạng khác nếu cần
           .ForMember(dest => dest.AttachmentUrl, opt => opt.Ignore())
           .ForMember(dest => dest.OrderInPaper, opt => opt.MapFrom(src => src.OrderInPaper))
           .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Question.Description))
           .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Question.Answers));

        }
    }

}
