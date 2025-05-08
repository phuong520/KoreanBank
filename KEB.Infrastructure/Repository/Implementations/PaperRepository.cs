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
    public class PaperRepository : GenericRepository<Paper>, IPaperRepository
    {
        public PaperRepository(ExamBankContext context) : base(context)
        {
        }

        public async Task DeletePaperAsynce(Paper paper)
        {
            _context.Papers.Remove(paper);
            await _context.SaveChangesAsync();
        }
        public async Task<Paper> GetDetailAsync(Guid paperId)
        {
            return await _context.Papers
            .Include(p => p.PaperDetails)
                .ThenInclude(pq => pq.Question)
            .FirstOrDefaultAsync(p => p.Id == paperId);
        }
    }
}
