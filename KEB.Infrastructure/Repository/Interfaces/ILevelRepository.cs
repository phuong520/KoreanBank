using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository.Interfaces
{
    public interface ILevelRepository: IGenericReposistory<Level>
    {
        Task<Level?> GetLevelById(Guid id);
        Task<Level?> GetLevelByName(string name);
        Task<(bool IsSuccess, int RelatedQuestions, int RelatedExamTypes, int RelatedTopics)> DeleteLevel(Guid id);

    }
}
