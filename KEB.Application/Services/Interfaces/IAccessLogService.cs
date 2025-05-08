using KEB.Application.DTOs.SystemAccessLogDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IAccessLogService
    {
        Task<APIResponse<AddQuestionHistoryDto>> ViewImportQuestionHistory(ViewAddQuestionHistory request);
        Task<APIResponse<AccessLogDisplayDto>> GetAccessLogs(ViewAccessLog request);

    }
}
