using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.ImportQuestionTaskDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IAddQuestionTaskService
    {
        Task<APIResponse<TaskGeneralDisplayDTO>> GetImportQuestionTask(ViewTaskRequest request);
        Task<APIResponse<TaskGeneralDisplayDTO>> AssignImportQuestionTask(AssignTaskRequest request);
        Task<APIResponse<TaskGeneralDisplayDTO>> EditImportQuestionTask(EditTaskRequest request);
        Task<APIResponse<TaskGeneralDisplayDTO>> DeleteImportQuestionTask(Delete request);
        Task<APIResponse<TaskGeneralDisplayDTO>> ChangeTaskDeadline(ChangeTaskDeadlineRequest request);
        Task<APIResponse<TaskFullDisplayDTO>> GetTaskDetail(Guid id);

    }
}
