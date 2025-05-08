using KEB.Domain.Entities;
using KEB.Infrastructure.Context;
using KEB.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository.Implementations
{
    public class SystemAccessLogRepository : GenericRepository<SystemAccessLog>, ISystemAccessLogRepository
    {
        public SystemAccessLogRepository(ExamBankContext context) : base(context)
        {
        }

        public async Task<SystemAccessLog> DeleteAsync(Guid id)
        {
            var entity = await _context.SystemAccessLogs.FindAsync(id);
            _context.SystemAccessLogs.Remove(entity);
            return entity;
        }
    }
}
