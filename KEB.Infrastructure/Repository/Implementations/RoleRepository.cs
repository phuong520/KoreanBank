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
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ExamBankContext context) : base(context)
        {
        }

        public async Task<string> GetRoleName(Guid id)
        {
            var result = await _context.Roles.FindAsync(id);
            return result?.RoleName ?? "";
        }
    }
}
