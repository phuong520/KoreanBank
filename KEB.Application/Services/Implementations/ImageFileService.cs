using KEB.Application.DTOs.ImageFileDTO;
using KEB.Application.Services.Interfaces;
using KEB.Domain.Entities;
using KEB.Infrastructure.UnitOfWorks;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Services.Implementations
{
    public class ImageFileService : IImageFileService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ImageFileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ImageFileDto> DownloadFileAsync(Guid id)
        {
            var image = await _unitOfWork.ImageFiles.GetByIdAsync(id);
            if (image == null)
            {
                return null;
            }
            return new ImageFileDto
            {
                FileName = image.FileName,
                FileData = image.FileData,
                ContentType = image.ContentType
            };
        }

        public async Task<List<ImageFileDto>> GetAllFileAsync()
        {
            throw new NotImplementedException();
        }

        public async Task UploadFileAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var image = new ImageFile
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                FileData = memoryStream.ToArray(),
                ContentType = file.ContentType
            };
            await _unitOfWork.ImageFiles.AddAsync(image);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
