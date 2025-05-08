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
    public class ImageFileIRepository : GenericRepository<ImageFile>, IImageFileRepository
    {
        public ImageFileIRepository(ExamBankContext context) : base(context)
        {
        }
    }
}
