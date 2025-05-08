using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository.Interfaces
{
    public interface ISystemAccessLogRepository :IGenericReposistory<SystemAccessLog>
    {
        Task<SystemAccessLog> DeleteAsync(Guid id);
    }
}
