using KEB.Application.DTOs.QuestionTypeDTO;
using KEB.Domain.Entities;
using KEB.Domain.Enums;


namespace KEB.Application.Services.Interfaces
{
    public interface IQuestionTypeService
    {
        Task<APIResponse<QuestionType>> GetQuestionTypesBySkillAsync(Skill skill);
        Task<APIResponse<QuestionTypeDisplayDto>> GetAllQuestionTypes(GetQuestionType request);
        Task<APIResponse<QuestionTypeDisplayDto>> GetQuestionType(Guid id);
        Task<APIResponse<QuestionTypeDisplayDto>> AddQuestionType(QuestionTypeCreateDto questionTypeCreateDTO, string ipAddress);
        Task<APIResponse<QuestionTypeDisplayDto>> UpdateQuestionType(QuestionTypeUpdateDto questionTypeUpdateDTO, string ipAddress);
        Task<APIResponse<QuestionTypeDisplayDto>> DeleteQuestionType(QuestionTypeDeleteDto questionTypeDeleteDTO, string ipAddress);

    }
}
