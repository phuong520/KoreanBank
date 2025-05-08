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
    public class TopicRepository : GenericRepository<Topic>, ITopicRepository
    {
        public TopicRepository(ExamBankContext context) : base(context)
        {
        }

        public async Task<(bool IsSuccess, int RelatedQuestions, int RelatedLevels, int RelatedConstraints)> DeleteTopicAsync(Topic topic)
        {
            // Đếm số câu hỏi liên kết
            var relatedQuestions = await _context.Questions
                .CountAsync(q => q.Id == topic.Id);

            // Đếm số LevelDetail liên kết
            var relatedLevels = await _context.LevelDetails
                .CountAsync(ld => ld.TopicId == topic.Id);

            // Đếm số ConstraintDetail liên kết
            var relatedConstraints = await _context.ConstraintDetails
                .CountAsync(cd => cd.TopicId == topic.Id);

            // Nếu có ràng buộc, không cho xóa
            if (relatedQuestions > 0 || relatedLevels > 0 || relatedConstraints > 0)
            {
                return (false, relatedQuestions, relatedLevels, relatedConstraints);
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return (true, 0, 0, 0);
        }

        public async Task<Topic?> GetByName(string name)
        {
            return await _context.Topics
              .FirstOrDefaultAsync(t => t.TopicName.ToLower() == name.ToLower());
        }
    }
}
