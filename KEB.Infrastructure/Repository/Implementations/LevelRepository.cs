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
    public class LevelRepository : GenericRepository<Level>, ILevelRepository
    {
        public LevelRepository(ExamBankContext context) : base(context)
        {
        }

        public async Task<(bool IsSuccess, int RelatedQuestions, int RelatedExamTypes, int RelatedTopics)> DeleteLevel(Guid id)
        {
            var level = await _context.Levels.FindAsync(id);
            if (level == null)
                throw new KeyNotFoundException("Không tìm thấy Level");

            // Load liên quan
            var details = await _context.LevelDetails
                .Where(x => x.LevelId == id)
                .Include(x => x.Questions)
                .ToListAsync();

            var examTypes = await _context.ExamTypes
                .Where(x => x.LevelId == id)
                .ToListAsync();

            int relatedQuestions = details.Sum(d => d.Questions.Count);
            int relatedExamTypes = examTypes.Count;
            int relatedTopics = details.Select(d => d.TopicId).Distinct().Count(); 

            if (relatedQuestions == 0 && relatedExamTypes == 0 && relatedTopics == 0)
            {
                _context.LevelDetails.RemoveRange(details);
                _context.Levels.Remove(level);
                await _context.SaveChangesAsync(); 
                return (true, 0, 0, 0);
            }

            return (false, relatedQuestions, relatedExamTypes, relatedTopics);
        }

        public async Task<Level?> GetLevelById(Guid id)
        {
            return await _context.Levels.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Level?> GetLevelByName(string name)
        {
            return await _context.Levels.FirstOrDefaultAsync(l => l.LevelName.Trim().ToLower().Equals(name.Trim().ToLower()));
        }
    }
}
