using KEB.Domain.Entities;
using KEB.Infrastructure.Context;
using KEB.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository.Implementations
{
    public class AddQuestionTaskRepository : GenericRepository<AddQuestionTask>, IAddQuestionTaskRepository
    {
        public AddQuestionTaskRepository(ExamBankContext context) : base(context)
        {
            
        }
        //lấy task co index lớn nhất
        public int FinalTaskIndex
        {
            get
            {
                
                IEnumerable<string> nameList = _context.AddQuestions.Select(x => x.TaskName);
                int biggestIndex = 0;
                foreach (var name in nameList)
                {
                    var index = int.Parse(name.Split("_").Last());
                    biggestIndex = Math.Max(biggestIndex, index);
                }
                return biggestIndex;
            }
        }

        public async Task<(AddQuestionTask? DeletedTask, int RelatedQuestions)> DeleteTaskAsync(Guid taskId)
        {
            var task = await _context.AddQuestions
                        .Include(x=>x.Questions)
                        .Include(x=>x.User)
                        .FirstOrDefaultAsync(x=>x.Id == taskId);

            if (task == null)
                return (null, 0);

            var questions = task.Questions;
            if (!questions.Any())
            {
                _context.AddQuestions.Remove(task);
                await _context.SaveChangesAsync();
            }
            return (task, questions.Count());
        }

        public async Task<(AddQuestionTask? DeletedTask, int RelatedQuestions)> DeleteTaskButKeepQuestionsAsync(Guid taskId)
        {
            var task = await _context.AddQuestions.Include(x=>x.Questions)
                        .FirstOrDefaultAsync (x=>x.Id == taskId);
            if(task == null)
                return (null, 0);
            var questions = task.Questions;
            foreach (var question in questions)
            {
                question.AddQuestionTask = null;
                question.TaskId = null; //bỏ liên kết với question
            }
            _context.AddQuestions.Remove(task);
            await _context.SaveChangesAsync();
            return (task, questions.Count());
        }
    }
}
