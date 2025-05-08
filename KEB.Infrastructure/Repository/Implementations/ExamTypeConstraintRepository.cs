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
    public class ExamTypeConstraintRepository : GenericRepository<ExamTypeConstraint>, IExamTypeConstraintRepository
    {
        public ExamTypeConstraintRepository(ExamBankContext context) : base(context)
        {
            
        }

        public async Task DeleteConstraintAndDetails(ExamTypeConstraint constraint)
        {
            //load tat ca cac bang lien quan
            var details = await _context.ConstraintDetails
                .Where(d => d.ExamTypeConstraintId == constraint.Id)
                .ToListAsync();

            _context.ConstraintDetails.RemoveRange(details);
            _context.ExamTypeConstraints.Remove(constraint);

            await _context.SaveChangesAsync();
        }
    }
}
