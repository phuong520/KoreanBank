using KEB.Application.DTOs.ExamPaperDTO;
using KEB.Application.DTOs.SystemAccessLogDTO;
using KEB.Application.DTOs.ExamTypeConstraintDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IExamPaperService
    {
        Task<APIResponse<PaperGeneralDisplayDTO>> ChangePaperStatus(ChangePaperStatusRequest request);
        Task<APIResponse<PaperGeneralDisplayDTO>> EditExamPaper(EditPaperDetailRequest request);
        Task<APIResponse<PaperDetailDisplayDTO>> UserGenerateExamPapers(Guid examId, Guid requestedUserId, string? ipAddress);
        Task<APIResponse<object>> LeaveCommentOnPaper(LeaveCommentRequest request);
        Task<APIResponse<AccessLogDisplayDto>> ViewActivitiesOnPaper(Guid paperId);
        Task<APIResponse<PaperDetailDisplayDTO>> ViewExamPaperDetail(ViewPaperDetailRequest request);
        Task<APIResponse<PaperGeneralDisplayDTO>> ViewExamPapers(ViewExamPapersListRequest request);
        Task<APIResponse<string>> GetUrlOfPaperAudio(ViewPaperDetailRequest request);
        Task<APIResponse<string>> GetUrlOfPaperContentPdf(ViewPaperDetailRequest request);
        Task<APIResponse<object>> UploadExamMaterials(Guid examId, Guid? requestedUserId = null, string ipAddress = "");
        //Task<APIResponse<PaperDetailDisplayDTO>> GenerateExamPapers(GenerateExamPaperRequest request);
    }
}
