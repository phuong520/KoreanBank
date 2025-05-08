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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ExamBankContext context) : base(context)
        {

        }

        public Task<string> GetAsync(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();

        }

        public async Task<User?> GetUserById(Guid userId)
        {
            return await _context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
