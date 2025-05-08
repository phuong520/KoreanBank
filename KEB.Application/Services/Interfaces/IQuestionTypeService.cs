using KEB.Application.DTOs.QuestionTypeDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IQuestionTypeService
    {
        Task<APIResponse<QuestionTypeDisplayDto>> GetAllQuestionTypes(GetQuestionType request);
        Task<APIResponse<QuestionTypeDisplayDto>> GetQuestionType(Guid id);
        Task<APIResponse<QuestionTypeDisplayDto>> AddQuestionType(QuestionTypeCreateDto questionTypeCreateDTO, string ipAddress);
        Task<APIResponse<QuestionTypeDisplayDto>> UpdateQuestionType(QuestionTypeUpdateDto questionTypeUpdateDTO, string ipAddress);
        Task<APIResponse<QuestionTypeDisplayDto>> DeleteQuestionType(QuestionTypeDeleteDto questionTypeDeleteDTO, string ipAddress);

    }
}
