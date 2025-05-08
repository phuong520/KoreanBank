using KEB.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Repository.Interfaces
{
    public interface IReferenceRepository  :IGenericReposistory<References>
    {
        Task<References?> GetUniqueRefAsync(string name, int year);
        Task<(bool IsSuccess, int RelatedQuestions)> DeleteRefAsync(References reference);

    }
}
