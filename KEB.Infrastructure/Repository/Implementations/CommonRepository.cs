using KEB.Domain.Entities;
using KEB.Infrastructure.Context;
using KEB.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository.Implementations
{
    public class CommonRepository : GenericRepository<User>, ICommonRepository
    {
        public CommonRepository(ExamBankContext context) : base(context)
        {
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            // Hash mật khẩu người dùng nhập vào
            string hashedPassword = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)))
                                              .Replace("-", "").ToLower();

            // Tìm người dùng với mật khẩu đã băm
            var user = await _context.Users
                                     .Include(u => u.Role)
                                     .FirstOrDefaultAsync(x =>
                                         x.UserName.ToLower() == username.ToLower() &&
                                         x.Password == hashedPassword);
            if (user == null)
            {
                return null;
            }
            // Nếu không có role, gán role mặc định là "Unknown"
            if (user.Role == null)
            {
                user.Role = new Role { RoleName = "Unknown" };  // Gán giá trị mặc định cho Role nếu không có
            }
            return user;

        }
       
    }
}
