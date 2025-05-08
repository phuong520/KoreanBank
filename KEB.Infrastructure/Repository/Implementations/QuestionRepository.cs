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
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        public QuestionRepository(ExamBankContext context) : base(context)
        {
        }

        public async Task<(Question Question, int Answers)> DeleteQuestionAsync(Question question)
        {
            var answers = _context.Answers.Where(a => a.QuestionId == question.Id).ToList();
            _context.RemoveRange(answers);
            _context.Remove(question);
            await _context.SaveChangesAsync();
            return (question, answers.Count);
        }

        public async Task<Question?> GetQuestionDetailByIdAsync(Guid id)
        {
            return await _context.Questions
        .Include(q => q.LevelDetail)
            .ThenInclude(ld => ld.Level)
        .Include(q => q.LevelDetail)
            .ThenInclude(ld => ld.Topic)
        .Include(q => q.QuestionType)
        .Include(q => q.References)
        .Include(q => q.Answers)
        //.Include(q => q.CreatedBy)
        //.Include(q => q.UpdatedBy)
        .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<List<Question>> GetRandomQuestionsAsync(Guid levelId, Guid questionTypeId, int quantity)
        {
            return await _context.Questions
          .Where(q => q.LevelDetailId == levelId && q.QuestionTypeId == questionTypeId)
          .OrderBy(q => Guid.NewGuid()) // Random
          .Take(quantity)
          .ToListAsync();
        }

        public int MinimumTotalDurationOfNListeningQuestion(int count)
        {
            var questions = _context.Questions
                .Where(x => x.QuestionType.Skill == Domain.Enums.Skill.Nghe && x.AttachmentDuration.HasValue)
                .OrderBy(x => x.AttachmentDuration)
                .Take(count)
                .ToList();

            if (questions.Count < count)
            {
                throw new ArgumentException($"Không đủ {count} câu hỏi nghe có thời lượng trong ngân hàng câu hỏi. Hiện tại chỉ có {questions.Count} câu hỏi.");
            }

            var totalDuration = questions.Sum(x => x.AttachmentDuration ?? 0);
            if (totalDuration == 0)
            {
                throw new ArgumentException("Không thể tính thời lượng tối thiểu vì các câu hỏi nghe không có thời lượng.");
            }

            return totalDuration;
        }

    }
}
