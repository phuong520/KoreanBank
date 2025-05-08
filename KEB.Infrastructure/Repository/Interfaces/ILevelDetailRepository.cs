using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository.Interfaces
{
    public interface ILevelDetailRepository:IGenericReposistory<LevelDetail>
    {

        Task<LevelDetail?> GetByLevelAndTopicNameAsync(string levelNameDashTopicName);
        Task<(bool Deleted, int RelatedQuestion)> RemoveLevelDetail(Guid levelId, Guid topicId);
        Task<(bool Success, int DeletedRecords)> RemoveRangeLevelDetail(IEnumerable<LevelDetail> levelDetails);
        Task<List<LevelDetail>> GetDetailByLevelId(Guid levelId);
        Task<Guid> GetByLevelAndTopicIdAsync(Guid levelId, Guid topicId);

    }
}
