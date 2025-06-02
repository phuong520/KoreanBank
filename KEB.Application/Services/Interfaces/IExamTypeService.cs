using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ExamTypeDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IExamTypeService
    {
        Task<APIResponse<ExamTypeComplexDisplayDTO>> AddExamTypeAsync(AddExamTypeRequest request);
        Task<APIResponse<ExamTypeComplexDisplayDTO>> DeleteExamTypeAsync(Delete request);
        Task<APIResponse<object>> EditExamTypeAsync(FullEditExamTypeRequest request);
        Task<APIResponse<ExamTypeComplexDisplayDTO>> GetExamTypeDetails(Guid examTypeId);
        Task<APIResponse<ExamTypeGeneralDisplayDTO>> GetExamTypesAsync(Pagination request);

    }
}
