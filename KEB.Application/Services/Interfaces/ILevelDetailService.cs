using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.LevelTopicDetailDTO;
using KEB.Application.DTOs.TopicDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface ILevelDetailService
    {
        Task<APIResponse<TopicDisplayDto>> AssignLevelsToTopic(AddValuesToEntityRequest request);
        Task<APIResponse<LevelDisplayBriefDTO>> AssignTopicsToLevel(AddValuesToEntityRequest request);
        Task<APIResponse<DetailDisplayDTO>> DeleteLevelDetail(DeleteDetailRequest request);
        Task<APIResponse<DetailDisplayDTO>> GetDetailByLevelId(Guid levelId);
        Task<APIResponse<DetailDisplayDTO>> GetDetailByLevelTopic(Guid levelId, Guid topicId);
    }
}
