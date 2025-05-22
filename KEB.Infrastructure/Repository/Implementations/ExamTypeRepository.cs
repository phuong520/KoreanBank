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
    public class ExamTypeRepository : GenericRepository<ExamType>, IExamTypeRepository
    {
        public ExamTypeRepository(ExamBankContext context) : base(context)
        {

        }
        
        public async Task DeleteExamType(ExamType examType, bool deleteConstraintOnly)
        {
            // Load các Constraint kèm ConstraintDetails
            var constraints = await _context.ExamTypeConstraints
                .Include(c => c.ConstraintDetails)
                .Where(c => c.ExamTypeId == examType.Id)
                .ToListAsync();

            foreach (var constraint in constraints)
            {
                _context.ConstraintDetails.RemoveRange(constraint.ConstraintDetails);
            }

            _context.ExamTypeConstraints.RemoveRange(constraints);

            if (!deleteConstraintOnly)
            {
                _context.ExamTypes.Remove(examType);
            }

            await _context.SaveChangesAsync();
        }
  
    }
}
