using KEB.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Utils
{
    public static class GetAttachFile
    {
        public static async Task<ImageFile> GetImageFile(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            Guid idImage = Guid.NewGuid();
            var image = new ImageFile
            {
                Id = idImage,
                FileName = file.FileName,
                FileData = memoryStream.ToArray(),
                ContentType = file.ContentType
            };
            return image;
        }
    }
}
