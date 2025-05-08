using KEB.Domain.Entities;
using KEB.Domain.Enums;
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
    public class QuestionTypeRepository : GenericRepository<QuestionType>, IQuestionTypeRepository
    {
        public QuestionTypeRepository(ExamBankContext context) : base(context)
        {
        }

        public async Task<bool> DeleteAsync(QuestionType targetQuestionType)
        {
            var hasQuestions = await _context.Questions
                .AnyAsync(q => q.QuestionTypeId == targetQuestionType.Id);
            if (hasQuestions) return false;

            _context.QuestionTypes.Remove(targetQuestionType);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<QuestionType?> GetQuestionTypeHasNameAndSkill(string name, Skill skill)
        {
            return await _context.QuestionTypes
            .SingleOrDefaultAsync(qt => qt.TypeName == name && qt.Skill == skill);
        }

        public async Task<bool> UpdateAsync(QuestionType targetQuestionType)
        {
            _context.QuestionTypes.Update(targetQuestionType);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
