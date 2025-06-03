using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.LevelDTO;
using KEB.Application.DTOs.LevelTopicDetailDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface ILevelService
    {
        Task<APIResponse<DetailDisplayDTO>> GetLevelNameDashTopic();
        Task<APIResponse<LevelDisplayDetailDTO>> GetLevelDetails(Guid levelId);

        Task<APIResponse<LevelDisplayBriefDTO>> GetAllLevels(Pagination request);
        Task<APIResponse<LevelDisplayBriefDTO>> GetLevel(Guid id);

        Task<APIResponse<LevelDisplayDetailDTO>> AddLevel(AddLevelRequest request, string ipAddress);
        Task<APIResponse<LevelDisplayBriefDTO>> DeleteLevel(Delete request, string ipAddress);
        Task<APIResponse<LevelDisplayBriefDTO>> RenameLevel(RenameLevelRequest request, string ipAddress);
      

    }
}
