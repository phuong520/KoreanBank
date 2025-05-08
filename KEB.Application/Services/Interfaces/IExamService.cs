using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ExamDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IExamService
    {
        Task<APIResponse<object>> AddExamAsync(AddExamRequest request);
        Task<APIResponse<object>> EditExamAsync(EditExamRequest request);
        Task<APIResponse<object>> GetExamAsync(Guid? id);
        Task<APIResponse<object>> DeleteExamAsync(Delete request);
        Task<APIResponse<ExamAsTaskDisplayDTO>> GetExamAsTask(GetExamAsTaskRequest request);
        Task<APIResponse<object>> UnhideExam(Guid? requestedUserId, Guid examId, string? ipAddress = "::1", string keyword = "");
        Task<APIResponse<object>> HideExam(Guid? requestedUserId, Guid examId, string? ipAddress = "::1", string keyword = "");
        Task<APIResponse<object>> UserSuspendExam(Guid requestedUserId, Guid examId, string? reason, string ipAddress = "");

    }
}
