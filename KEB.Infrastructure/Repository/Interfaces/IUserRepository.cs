using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository.Interfaces
{
    public interface IUserRepository: IGenericReposistory<User>
    {
        Task<User?> GetUserById(Guid userId);
        Task<User?> GetUserByEmail(string email);
        Task UpdateAsync(User user);
        Task<string> GetAsync(Func<object, bool> value);
    }
}
