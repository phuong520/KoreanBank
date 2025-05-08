//using Azure;
//using Azure.Storage.Blobs;
//using Azure.Storage.Blobs.Models;
//using KEB.Infrastructure.ExternalService.IExternalInterfaces;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace KEB.Infrastructure.ExternalService.IExternalImplementation
//{
//    public class FileService : IFileService
//    {
//        private readonly BlobServiceClient _blobServiceClient;
//        public FileService(BlobServiceClient blobServiceClient)
//        {
//            _blobServiceClient = blobServiceClient;
//        }
//        public async Task DeleteBlob(string blobName, string containerName)
//        {
//            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
//            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
//            await blobClient.DeleteIfExistsAsync();
//        }

//        public async Task DeleteBlobByUrl(string blobUrl)
//        {
//            // Create a BlobClient directly from the URL
//            BlobClient blobClient = new(new Uri(blobUrl));

//            // Delete the blob if it exists
//            await blobClient.DeleteIfExistsAsync();
//        }

//        public async Task<Stream> Get(string containerName, string blobName)
//        {
//            var containerInstance = _blobServiceClient.GetBlobContainerClient(containerName);
//            var blobInstance = containerInstance.GetBlobClient(blobName);
//            BlobDownloadInfo downloadContent = await blobInstance.DownloadAsync();
//            return downloadContent.Content;
//        }

//        public async Task<string> GetBlob(string? blobName, string containerName)
//        {
//            if (string.IsNullOrEmpty(blobName)) return "";
//            try
//            {
//                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
//                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

//                if (await blobClient.ExistsAsync())
//                {
//                    Console.WriteLine($"Get blob URL: {blobClient.Uri}");
//                    return blobClient.Uri.ToString();
//                }
//                else
//                {
//                    Console.WriteLine("Blob does not exist.");
//                }
//            }
//            catch (RequestFailedException ex)
//            {
//                Console.WriteLine($"Error: {ex.Message}");
//            }
//            return "";
//        }

//        public async Task<List<string>> GetBlobsInContainer(string containerName)
//        {
//            List<string> result = [];
//            var containerInstance = _blobServiceClient.GetBlobContainerClient(containerName);
//            await foreach (var blobItem in containerInstance.GetBlobsAsync())
//            {
//                result.Add(blobItem.Name);
//            }
//            return result;
//        }

//        public async Task<string> Upload(string container, IFormFile file, string? fileName = "")
//        {
//            var containerInstance = _blobServiceClient.GetBlobContainerClient(container);
//            var blobInstance = containerInstance.GetBlobClient(fileName ?? file.FileName);
//            try
//            {
//                var uploadResult = await blobInstance.UploadAsync(file.OpenReadStream());

//                string blobUrl = blobInstance.Uri.ToString();

//                return blobUrl;
//            }
//            catch (Exception ex)
//            {
//                await Console.Out.WriteLineAsync(ex.Message);
//                return blobInstance.Uri.ToString();
//            }
//        }

//        public async Task<string> Upload(string containerName, MemoryStream stream, string blobName)
//        {
//            var containerInstance = _blobServiceClient.GetBlobContainerClient(containerName);
//            var blobInstance = containerInstance.GetBlobClient(blobName);
//            await blobInstance.UploadAsync(stream);

//            string blobUrl = blobInstance.Uri.ToString();

//            return blobUrl;
//        }

//        public async Task<string> Upload(string containerName, string path, string blobName)
//        {
//            var containerInstance = _blobServiceClient.GetBlobContainerClient(containerName);
//            var blobInstance = containerInstance.GetBlobClient(blobName);
//            await blobInstance.UploadAsync(path);

//            string blobUrl = blobInstance.Uri.ToString();

//            return blobUrl;
//        }
//    }
//}
