using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository.Interfaces
{
    public interface IAddQuestionTaskRepository : IGenericReposistory<AddQuestionTask>
    {
        int FinalTaskIndex { get; }
        Task<(AddQuestionTask? DeletedTask, int RelatedQuestions)> DeleteTaskAsync(Guid taskId);
        Task<(AddQuestionTask? DeletedTask, int RelatedQuestions)> DeleteTaskButKeepQuestionsAsync(Guid taskId);
    }
}
