using KEB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository.Interfaces
{
    public interface IPaperRepository : IGenericReposistory<Paper>
    {
        Task DeletePaperAsynce(Paper paper);
        Task<Paper> GetDetailAsync(Guid paperId);

    }
}
