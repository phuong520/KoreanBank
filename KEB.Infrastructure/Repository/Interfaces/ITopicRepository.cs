using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository.Interfaces
{
    public interface ITopicRepository: IGenericReposistory<Topic>
    {
        Task<Topic?> GetByName(string name);
        Task<(bool IsSuccess, int RelatedQuestions, int RelatedLevels, int RelatedConstraints)> DeleteTopicAsync(Topic topic);
    }
}
