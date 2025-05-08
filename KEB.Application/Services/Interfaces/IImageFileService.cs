using KEB.Application.DTOs.ImageFileDTO;
using KEB.Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Interfaces
{
    public interface IImageFileService 
    {
        Task UploadFileAsync(IFormFile file);
        Task<ImageFileDto> DownloadFileAsync(Guid id);

        Task<List<ImageFileDto>> GetAllFileAsync();
    }
}
