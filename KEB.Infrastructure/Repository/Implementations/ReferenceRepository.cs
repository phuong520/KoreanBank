
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
    public class ReferenceRepository : GenericRepository<References>, IReferenceRepository
    {
        public ReferenceRepository(ExamBankContext context) : base(context)
        {

        }

        public async Task<(bool IsSuccess, int RelatedQuestions)> DeleteRefAsync(References reference)
        {
            int relatedQuestions = await _context.Questions
                    .CountAsync(q => q.ReferenceId == reference.Id);

            if (relatedQuestions > 0)
            {
                return (false, relatedQuestions);
            }

            _context.References.Remove(reference);
            await _context.SaveChangesAsync();
            return (true, 0);
        }

        public async Task<References?> GetUniqueRefAsync(string name, int year)
        {
            return await _context.References
                .FirstOrDefaultAsync(r => r.ReferenceName == name && r.PublishedYear == year);
        }
    }
}
