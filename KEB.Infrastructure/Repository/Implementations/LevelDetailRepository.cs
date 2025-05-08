using KEB.Domain.Entities;
using KEB.Infrastructure.Context;
using KEB.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository.Implementations
{
    public class LevelDetailRepository : GenericRepository<LevelDetail>, ILevelDetailRepository
    {
        public LevelDetailRepository(ExamBankContext context) : base(context)
        {
        }
        public async Task<Guid> GetByLevelAndTopicIdAsync(Guid levelId, Guid topicId)
        {
             var detail = await _context.LevelDetails
                .FirstOrDefaultAsync(ld => ld.LevelId == levelId && ld.TopicId == topicId);
            return detail.Id;
        }
        public async Task<LevelDetail?> GetByLevelAndTopicNameAsync(string levelNameDashTopicName)
        {
            var parts = levelNameDashTopicName.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) return null;

            var levelName = parts[0].Trim();
            var topicName = parts[1].Trim();

            return await _context.LevelDetails
                .Include(ld => ld.Level)
                .Include(ld => ld.Topic)
                .FirstOrDefaultAsync(ld => ld.Level.LevelName == levelName && ld.Topic.TopicName == topicName);
        
        }

        public async Task<List<LevelDetail>> GetDetailByLevelId(Guid levelId)
        {
            var levelDetail = await _context.LevelDetails.Where(x => x.LevelId == levelId)
                .Include(x=>x.Topic)
                .ToListAsync();
            return levelDetail;
        }

        public async Task<(bool Deleted, int RelatedQuestion)> RemoveLevelDetail(Guid levelId, Guid topicId)
        {
            var levelDetail = await _context.LevelDetails
            .Include(ld => ld.Questions)
            .SingleOrDefaultAsync(ld => ld.LevelId == levelId && ld.TopicId == topicId);

            if (levelDetail != null)
            {
                if (!levelDetail.Questions.Any())
                {
                    _context.LevelDetails.Remove(levelDetail);
                    await _context.SaveChangesAsync();
                    return (true, 0);
                }
                else
                {
                    return (false, levelDetail.Questions.Count());
                }
            }
            else
            {
                return (false, -1);
            }
        }

        public async Task<(bool Success, int DeletedRecords)> RemoveRangeLevelDetail(IEnumerable<LevelDetail> levelDetails)
        {
            if (levelDetails == null || !levelDetails.Any())
                return (false, 0);

            int count = 0;
            foreach (var levelDetail in levelDetails)
            {
                if (levelDetail.Questions == null || !levelDetail.Questions.Any())
                {
                    _context.LevelDetails.Remove(levelDetail);
                    count++;
                }
                else
                {
                    // Có câu hỏi liên quan, không xóa bất kỳ gì, rollback logic
                    return (false, -1);
                }
            }

            await _context.SaveChangesAsync();
            return (true, count);
        }
    }
}
