using KEB.Application.DTOs.Common;
using KEB.Application.DTOs.TopicDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface ITopicService
    {
        Task<APIResponse<TopicDisplayDto>> GetTopic(Guid topicId);
        Task<APIResponse<TopicDisplayDto>> GetAllTopics(Guid? levelId = null, bool includedSoftDeleted = false, int page = 1, int size = 10);
        Task<APIResponse<TopicDetailDto>> GetTopicDetails(Guid topicId);
        Task<APIResponse<TopicDisplayDto>> AddNewTopic(AddTopicDto request);
        Task<APIResponse<TopicDisplayDto>> DeleteTopic(Delete request);
        Task<APIResponse<TopicDisplayDto>> RenameTopic(EditTopicDto request);

    }
}
