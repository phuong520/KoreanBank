using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.QuestionAddDTO;
using KEB.Application.DTOs.QuestionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<APIResponse<ChangeStatusResultDTO>> TeamLeadChangeQuestionStatus(ChangeQuestionStatusRequest request);
        Task<APIResponse<QuestionDetailDto>> TeamLeadDeleteQuestion(Delete request);
        Task<APIResponse<QuestionDetailDto>> GetQuestionDetailAsync(Guid questionId);
        Task<APIResponse<QuestionDisplayDto>> GetQuestionsListAsync(GetQuestionsRequest request);
        Task<APIResponse<ImportQuestionResultDTO>> UploadQuestionFromExcel(ImportQuestionFromExcelRequest request, string ipAddress);
        Task<APIResponse<object>> UpdateQuestion(UpdateQuestionRequest request);
        Task<APIResponse<QuestionDetailDto>> AddSingleQuestionAsync(AddSingleQuestionRequest request);
     

    }
}
