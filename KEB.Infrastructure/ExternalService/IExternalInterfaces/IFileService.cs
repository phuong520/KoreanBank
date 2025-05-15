using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.ExternalService.IExternalInterfaces
{
    public interface IFileService
    {
        Task<string> Upload(string container, IFormFile file, string? fileName = "");
        Task<string> Upload(string folderName, Stream fileStream, string fileName);
        Task<Stream> Get(string containerName, string blobName);
        Task<string> GetBlob(string? blobName, string containerName);
        Task DeleteBlob(string blobName, string containerName);

        Task<List<string>> GetBlobsInContainer(string containerName);
        Task DeleteBlobByUrl(string blobUrl);
        Task<string> Upload(string containerName, MemoryStream stream, string blobName);
        Task<string> Upload(string containerName, string path, string blobName);
    }
}
