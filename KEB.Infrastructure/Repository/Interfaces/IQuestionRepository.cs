using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository.Interfaces
{
    public interface IQuestionRepository :IGenericReposistory<Question>
    {
        Task<(Question Question, int Answers)> DeleteQuestionAsync(Question question);
        int MinimumTotalDurationOfNListeningQuestion(int count);
        Task<Question?> GetQuestionDetailByIdAsync(Guid id);
        Task<List<Question>> GetRandomQuestionsAsync(Guid levelId, Guid questionTypeId, int quantity);
    }
}
