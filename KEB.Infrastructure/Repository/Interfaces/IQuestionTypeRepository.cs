using KEB.Domain.Entities;
using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository.Interfaces
{
    public interface IQuestionTypeRepository : IGenericReposistory<QuestionType>
    {
 
        Task<bool> DeleteAsync(QuestionType targetQuestionType);
        Task<bool> UpdateAsync(QuestionType targetQuestionType);
        Task<QuestionType?> GetQuestionTypeHasNameAndSkill(string name, Skill skill);

    }
}
